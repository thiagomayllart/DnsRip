using DnsRip.Interfaces;
using DnsRip.Models;
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

                var question = new DnsQuestion(_request.Query, _request.Type);
                var request = new Request(question);
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

    public class AnswerRr : Rr
    {
        public AnswerRr(RecordReader br)
            : base(br)
        {
        }
    }

    public class AuthorityRr : Rr
    {
        public AuthorityRr(RecordReader br)
            : base(br)
        {
        }
    }

    public class AdditionalRr : Rr
    {
        public AdditionalRr(RecordReader br)
            : base(br)
        {
        }
    }

    public class Rr
    {
        public Rr(RecordReader rr)
        {
            TimeLived = 0;
            Name = rr.ReadDomainName();
            Type = (DnsRip.QueryType)rr.ReadUInt16();
            Class = (DnsRip.QueryClass)rr.ReadUInt16();
            Ttl = rr.ReadUInt32();
            RdLength = rr.ReadUInt16();
            Record = rr.ReadRecord(Type, RdLength);
            Record.Rr = this;
        }

        public int TimeLived;
        public string Name;
        public DnsRip.QueryType Type;
        public DnsRip.QueryClass Class;
        public ushort RdLength;
        public Record Record;

        private uint _ttl;

        public uint Ttl
        {
            get { return (uint)Math.Max(0, _ttl - TimeLived); }
            set { _ttl = value; }
        }
    }

    public abstract class Record
    {
        public Rr Rr;
    }

    public class RecordReader
    {
        public RecordReader(byte[] data)
        {
            _data = data;
            Position = 0;
        }

        public RecordReader(byte[] data, int position)
        {
            _data = data;
            Position = position;
        }

        private readonly byte[] _data;

        public int Position { get; set; }

        public string ReadDomainName()
        {
            var name = new StringBuilder();
            int length;

            while ((length = ReadByte()) != 0)
            {
                if ((length & 0xc0) == 0xc0)
                {
                    var newRecordReader = new RecordReader(_data, (length & 0x3f) << 8 | ReadByte());

                    name.Append(newRecordReader.ReadDomainName());

                    return name.ToString();
                }

                while (length > 0)
                {
                    name.Append(ReadChar());
                    length--;
                }

                name.Append('.');
            }

            return name.Length == 0 ? "." : name.ToString();
        }

        public string ReadString()
        {
            var length = ReadByte();
            var str = new StringBuilder();

            for (var intI = 0; intI < length; intI++)
                str.Append(ReadChar());

            return str.ToString();
        }

        public byte ReadByte()
        {
            return Position >= _data.Length ? (byte)0 : _data[Position++];
        }

        public byte[] ReadBytes(int intLength)
        {
            var list = new byte[intLength];

            for (var intI = 0; intI < intLength; intI++)
                list[intI] = ReadByte();

            return list;
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public ushort ReadUInt16()
        {
            return (ushort)(ReadByte() << 8 | ReadByte());
        }

        public ushort ReadUInt16(int offset)
        {
            Position += offset;

            return ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return (uint)(ReadUInt16() << 16 | ReadUInt16());
        }

        public Record ReadRecord(DnsRip.QueryType type, int length)
        {
            switch (type)
            {
                case DnsRip.QueryType.A:
                    return new RecordA(this);

                case DnsRip.QueryType.CNAME:
                    return new RecordCName(this);

                case DnsRip.QueryType.AAAA:
                    return new RecordAaaa(this);

                case DnsRip.QueryType.NS:
                    return new RecordNs(this);

                case DnsRip.QueryType.MX:
                    return new RecordMx(this);

                case DnsRip.QueryType.SOA:
                    return new RecordSoa(this);

                case DnsRip.QueryType.TXT:
                    return new RecordTxt(this, length);

                case DnsRip.QueryType.PTR:
                    return new RecordPtr(this);

                default:
                    return new RecordUnknown(this);
            }
        }
    }

    public class Request
    {
        public Request(DnsQuestion question)
        {
            Header = new Header1();
            _question = question;
        }

        public Header1 Header;

        private readonly DnsQuestion _question;

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(Header.Data);
                data.AddRange(_question.Data);
                return data.ToArray();
            }
        }
    }

    public class Header1
    {
        public Header1()
        {
            Id = (ushort)new Random().Next();
            OpCode = DnsRip.OpCode.Query;
            QdCount = 1;
            Rd = true;
        }

        public Header1(RecordReader rr)
        {
            Id = rr.ReadUInt16();
            Flags = rr.ReadUInt16();
            QdCount = rr.ReadUInt16();
            AnCount = rr.ReadUInt16();
            NsCount = rr.ReadUInt16();
            ArCount = rr.ReadUInt16();
        }

        public ushort Id;
        public ushort Flags;
        public ushort QdCount;
        public ushort AnCount;
        public ushort NsCount;
        public ushort ArCount;

        public DnsRip.OpCode OpCode
        {
            get { return (DnsRip.OpCode)GetBits(Flags, 11, 4); }
            set { Flags = SetBits(Flags, 11, 4, (ushort)value); }
        }

        public bool Rd
        {
            get
            {
                return GetBits(Flags, 8, 1) == 1;
            }
            set
            {
                Flags = SetBits(Flags, 8, 1, value);
            }
        }

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(DnsRip.Tools.ToNetByteOrder(Id));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(Flags));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(QdCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(AnCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(NsCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(ArCount));
                return data.ToArray();
            }
        }

        private static ushort GetBits(ushort oldValue, int position, int length)
        {
            if (length <= 0 || position >= 16)
                return 0;

            var mask = (2 << (length - 1)) - 1;

            return (ushort)((oldValue >> position) & mask);
        }

        private static ushort SetBits(ushort oldValue, int position, int length, ushort newValue)
        {
            if (length <= 0 || position >= 16)
                return oldValue;

            var mask = (2 << (length - 1)) - 1;

            oldValue &= (ushort)~(mask << position);
            oldValue |= (ushort)((newValue & mask) << position);

            return oldValue;
        }

        private static ushort SetBits(ushort oldValue, int position, int length, bool blnValue)
        {
            return SetBits(oldValue, position, length, blnValue ? (ushort)1 : (ushort)0);
        }
    }

    public class RecordA : Record
    {
        public RecordA(RecordReader recordReader)
        {
            Address = new IPAddress(recordReader.ReadBytes(4));
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

        public RecordCName(RecordReader rr)
        {
            CName = rr.ReadDomainName();
        }

        public override string ToString()
        {
            return CName;
        }
    }

    public class RecordAaaa : Record
    {
        public IPAddress Address;

        public RecordAaaa(RecordReader rr)
        {
            IPAddress.TryParse(
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}:" +
                $"{rr.ReadUInt16():x}",
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

        public RecordNs(RecordReader rr)
        {
            NsDName = rr.ReadDomainName();
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

        public RecordMx(RecordReader rr)
        {
            Preference = rr.ReadUInt16();
            Exchange = rr.ReadDomainName();
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

        public RecordSoa(RecordReader rr)
        {
            MName = rr.ReadDomainName();
            RName = rr.ReadDomainName();
            Serial = rr.ReadUInt32();
            Refresh = rr.ReadUInt32();
            Retry = rr.ReadUInt32();
            Expire = rr.ReadUInt32();
            Minimum = rr.ReadUInt32();
        }

        public override string ToString()
        {
            return $"{MName} {RName} {Serial} {Refresh} {Retry} {Expire} {Minimum}";
        }
    }

    public class RecordTxt : Record
    {
        public List<string> Txt;

        public RecordTxt(RecordReader rr, int length)
        {
            var pos = rr.Position;

            Txt = new List<string>();

            while (rr.Position - pos < length)
                Txt.Add(rr.ReadString());
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

        public RecordPtr(RecordReader rr)
        {
            PtrDName = rr.ReadDomainName();
        }

        public override string ToString()
        {
            return PtrDName;
        }
    }

    public class RecordUnknown : Record
    {
        public RecordUnknown(RecordReader rr)
        {
            var rdLength = rr.ReadUInt16(-2);
            RData = rr.ReadBytes(rdLength);
        }

        public byte[] RData;
    }
}