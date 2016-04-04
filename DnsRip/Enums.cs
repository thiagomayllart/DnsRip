// ReSharper disable InconsistentNaming

namespace DnsRip
{
    public partial class DnsRip
    {
        public enum InputType
        {
            Ip,
            Hostname,
            Invalid
        }

        public enum QueryType
        {
            A = 1,
            AAAA = 28,
            CNAME = 5,
            NS = 2,
            MX = 15,
            SOA = 6,
            TXT = 16,
            PTR = 12
        }
    }
}