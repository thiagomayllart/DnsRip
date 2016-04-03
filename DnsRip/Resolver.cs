using DnsRip.Interfaces;
using DnsRip.Models;
using DnsRip.Utilites;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DnsRip
{
    public partial class DnsRip
    {
        public class Resolver
        {
            public Resolver(IResolveRequest request, int retries = 3, int secondsTimeout = 1)
            {
                _request = request;
                _retries = retries;
                _secondsTimeout = secondsTimeout;
            }

            private readonly IResolveRequest _request;
            private readonly int _retries;
            private readonly int _secondsTimeout;

            public IEnumerable<ResolveResponse> Resolve()
            {
                if (_request.Type == QueryType.PTR && Tools.IsIp(_request.Query))
                    _request.Query = Tools.ToArpaRequest(_request.Query);

                var header = new DnsHeader();
                var question = new DnsQuestion(_request.Query, _request.Type);
                var request = new DnsRequest(question, header);
                var resolved = new List<ResolveResponse>();

                foreach (var server in _request.Servers)
                {
                    var attempts = 0;

                    while (attempts <= _retries)
                    {
                        attempts++;

                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _secondsTimeout * 1000);

                        try
                        {
                            socket.SendTo(request.Data, new IPEndPoint(IPAddress.Parse(server), 53));

                            var buffer = new byte[512];
                            var received = socket.Receive(buffer);
                            var data = new byte[received];

                            Array.Copy(buffer, data, received);

                            var dnsResponse = new DnsResponse(data);

                            foreach (var resp in dnsResponse.Answers)
                            {
                                resolved.Add(new ResolveResponse
                                {
                                    Server = server,
                                    Host = resp.Name,
                                    Type = resp.Type,
                                    Record = resp.Record.ToString(),
                                    Ttl = resp.Ttl
                                });
                            }

                            break;
                        }
                        catch
                        {
                            if (attempts >= 3)
                                throw;
                        }
                        finally
                        {
                            socket.Close();
                        }
                    }
                }

                return resolved;
            }
        }
    }

    public abstract class Record
    {
        public RecordReader Rr;
    }

    public class RecordA : Record
    {
        public RecordA(RecordHelper helper)
        {
            Address = new IPAddress(helper.ReadBytes(4));
        }

        public IPAddress Address;

        public override string ToString()
        {
            return Address.ToString();
        }
    }

    public class RecordCName : Record
    {
        public string CName;

        public RecordCName(RecordHelper helper)
        {
            CName = helper.ReadDomainName();
        }

        public override string ToString()
        {
            return CName;
        }
    }

    public class RecordAaaa : Record
    {
        public IPAddress Address;

        public RecordAaaa(RecordHelper helper)
        {
            IPAddress.TryParse(
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}:" +
                $"{helper.ReadUInt16():x}",
                out Address);
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }

    public class RecordNs : Record
    {
        public string NsDName;

        public RecordNs(RecordHelper helper)
        {
            NsDName = helper.ReadDomainName();
        }

        public override string ToString()
        {
            return NsDName;
        }
    }

    public class RecordMx : Record
    {
        public ushort Preference;
        public string Exchange;

        public RecordMx(RecordHelper helper)
        {
            Preference = helper.ReadUInt16();
            Exchange = helper.ReadDomainName();
        }

        public override string ToString()
        {
            return $"{Preference} {Exchange}";
        }
    }

    public class RecordSoa : Record
    {
        public string MName;
        public string RName;
        public uint Serial;
        public uint Refresh;
        public uint Retry;
        public uint Expire;
        public uint Minimum;

        public RecordSoa(RecordHelper helper)
        {
            MName = helper.ReadDomainName();
            RName = helper.ReadDomainName();
            Serial = helper.ReadUInt32();
            Refresh = helper.ReadUInt32();
            Retry = helper.ReadUInt32();
            Expire = helper.ReadUInt32();
            Minimum = helper.ReadUInt32();
        }

        public override string ToString()
        {
            return $"{MName} {RName} {Serial} {Refresh} {Retry} {Expire} {Minimum}";
        }
    }

    public class RecordTxt : Record
    {
        public List<string> Txt;

        public RecordTxt(RecordHelper helper, int length)
        {
            var pos = helper.Position;

            Txt = new List<string>();

            while (helper.Position - pos < length)
                Txt.Add(helper.ReadString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var txt in Txt)
                sb.AppendFormat("\"{0}\" ", txt);

            return sb.ToString().TrimEnd();
        }
    }

    public class RecordPtr : Record
    {
        public string PtrDName;

        public RecordPtr(RecordHelper helper)
        {
            PtrDName = helper.ReadDomainName();
        }

        public override string ToString()
        {
            return PtrDName;
        }
    }

    public class RecordUnknown : Record
    {
        public RecordUnknown(RecordHelper helper)
        {
            var rdLength = helper.ReadUInt16(-2);
            RData = helper.ReadBytes(rdLength);
        }

        public byte[] RData;
    }
}