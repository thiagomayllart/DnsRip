using DnsRip.Interfaces;
using DnsRip.Models;
using DnsRip.Utilites;
using System.Collections.Generic;
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
                var dnsRequest = GetDnsRequest(request);
                var resolved = new List<ResolveResponse>();

                foreach (var server in request.Servers)
                {
                    var attempts = 0;

                    while (attempts <= _retries)
                    {
                        attempts++;

                        try
                        {
                            using (var socket = new SocketHelper(dnsRequest, server, _secondsTimeout))
                            {
                                var data = socket.Send();
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
                            }

                            break;
                        }
                        catch (SocketException)
                        {
                            if (attempts >= 3)
                                throw;
                        }
                    }
                }

                return resolved;
            }

            private static DnsRequest GetDnsRequest(IResolveRequest request)
            {
                if (request.Type == QueryType.PTR && Tools.IsIp(request.Query))
                    request.Query = Tools.ToArpaRequest(request.Query);

                var dnsHeader = new DnsHeader();
                var dnsQuestion = new DnsQuestion(request.Query, request.Type);

                return new DnsRequest(dnsHeader, dnsQuestion);
            }
        }
    }
}