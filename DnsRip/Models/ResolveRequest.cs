using DnsRip.Interfaces;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class ResolveRequest : IResolveRequest
    {
        public string Query { get; set; }
        public IEnumerable<string> Servers { get; set; }
        public DnsRip.QueryType Type { get; set; }
    }
}