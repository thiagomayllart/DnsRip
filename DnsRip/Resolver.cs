using DnsRip.Extensions;
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
            public Resolver(ResolveOptions options)
            {
                _options = options;
                _validator = new Validator();
            }

            private readonly ResolveOptions _options;
            private readonly Validator _validator;

            public IEnumerable<ResolveResponse> Resolve(IResolveRequest request)
            {
                var dnsRequest = GetDnsRequest(request);
                var resolved = new List<ResolveResponse>();

                foreach (var server in _options.Servers)
                {
                    var attempts = 0;

                    while (attempts <= _options.Retries)
                    {
                        attempts++;

                        try
                        {
                            using (var socket = new SocketHelper(dnsRequest, server, _options.Timeout))
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

            private DnsRequest GetDnsRequest(IResolveRequest request)
            {
                if (request.Type == QueryType.PTR && _validator.IsIp(request.Query))
                    request.Query = request.Query.ToArpaRequest();

                var dnsHeader = new DnsHeader();
                var dnsQuestion = new DnsQuestion(request.Query, request.Type);

                return new DnsRequest(dnsHeader, dnsQuestion);
            }
        }
    }
}