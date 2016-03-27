using DnsRip.Interfaces;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsRipRequest : IDnsRipRequest
    {
        public string Query { get; set; }
        public IEnumerable<string> Servers { get; set; }
        public bool IsRecursive { get; set; }
        public DnsRip.QueryType Type { get; set; }
    }
}