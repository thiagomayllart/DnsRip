namespace DnsRip.Interfaces
{
    public interface IResolveRequest
    {
        string Query { get; set; }
        DnsRip.QueryType Type { get; set; }
    }
}