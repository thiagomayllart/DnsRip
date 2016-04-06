using DnsRip.Interfaces;
using DnsRip.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DnsRip
{
    public partial class DnsRip
    {
        public class Resolver
        {
            public Resolver(int retries = 3, int secondsTimeout = 1)
            {
                _retries = retries;
                _secondsTimeout = secondsTimeout;
            }

            private readonly int _retries;
            private readonly int _secondsTimeout;

            public IEnumerable<ResolveResponse> Resolve(IResolveRequest request)
            {
                if (request.Type == QueryType.PTR && Tools.IsIp(request.Query))
                    request.Query = Tools.ToArpaRequest(request.Query);

                var dnsHeader = new DnsHeader();
                var dnsQuestion = new DnsQuestion(request.Query, request.Type);
                var dnsRequest = new DnsRequest(dnsQuestion, dnsHeader);
                var resolved = new List<ResolveResponse>();

                foreach (var server in request.Servers)
                {
                    var attempts = 0;

                    while (attempts <= _retries)
                    {
                        attempts++;

                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _secondsTimeout * 1000);

                        try
                        {
                            socket.SendTo(dnsRequest.Data, new IPEndPoint(IPAddress.Parse(server), 53));

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
                        catch (SocketException)
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
}