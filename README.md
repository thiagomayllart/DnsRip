# DnsRip

DnsRip is a simple .Net component for doing DNS lookups. This project is based off of [Alphons van der Heijden’s article at CodeProject](http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C). 

## Usage

```csharp
var dnsRip = new DnsRip.Resolver("8.8.4.4");
var results = dnsRip.Resolve("google.com", DnsRip.QueryType.A);
```

```javascript
//results (serialized)
[{
    "Server": "8.8.4.4",
    "Host": "google.com",
    "Ttl": 299,
    "Type": 1,
    "Record": "216.58.192.174"
}]
```

Also includes a parsing method for normalizing hostnames and IPs. Handy for working with URLs.

```csharp
var parser = new DnsRip.Parser();
var result1 = parser.Parse("http://www.hostname.com:80");
var result2 = parser.Parse("  http://192.168.10.1");
var result3 = parser.Parse("http://[FE80:0000:0000:0000:0202:B3FF:FE1E:8329]:8080/  ");
```