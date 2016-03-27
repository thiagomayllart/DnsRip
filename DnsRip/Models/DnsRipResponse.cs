namespace DnsRip.Models
{
    public class DnsRipResponse
    {
        public string Host { get; set; }
        public uint Ttl { get; set; }
        public DnsRip.QueryType Type { get; set; }
        public string Record { get; set; }
    }
}