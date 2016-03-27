using System.Collections.Generic;

namespace DnsRip.Interfaces
{
    public interface IDnsRipRequest
    {
        string Query { get; set; }
        IEnumerable<string> Servers { get; set; }
        bool IsRecursive { get; set; }
        DnsRip.QueryType Type { get; set; }
    }
}