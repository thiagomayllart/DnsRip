# DnsRip

DnsRip is a simple .Net component for doing DNS lookups. Currently only supports the follow record types: **A, AAAA, CNAME, NS, MX, SOA, TXT, PTR, and ANY**.

## Usage

```csharp
var dnsRip = new DnsRip.Resolver("8.8.4.4");
var results1 = dnsRip.Resolve("google.com", DnsRip.QueryType.A);
var results2 = dnsRip.Resolve("www.yahoo.com", DnsRip.QueryType.CNAME);
var results3 = dnsRip.Resolve("microsoft.com", DnsRip.QueryType.MX);
```

```javascript
//results1 (serialized)

[{
    "Server": "8.8.4.4",
    "Host": "google.com",
    "Ttl": 216,
    "Type": DnsRip.QueryType.A,
    "Record": "216.58.192.238"
}]

//results2 (serialized)

[{
    "Server": "8.8.4.4",
    "Host": "www.yahoo.com",
    "Ttl": 242,
    "Type": DnsRip.QueryType.CNAME,
    "Record": "fd-fp3.wg1.b.yahoo.com."
}]

//results3 (serialized)

[{
    "Server": "8.8.4.4",
    "Host": "microsoft.com",
    "Ttl": 1437,
    "Type": DnsRip.QueryType.MX,
    "Record": "10 microsoft-com.mail.protection.outlook.com."
}]
```

Also includes a parsing method for normalizing hostnames and IPs. Useful for working with URLs.

```csharp
var parser = new DnsRip.Parser();
var result1 = parser.Parse("http://www.hostname.com:80");
var result2 = parser.Parse("  http://192.168.10.1");
var result3 = parser.Parse("http://[FE80:0000:0000:0000:0202:B3FF:FE1E:8329]:8080/  ");
var result4 = parser.Parse("random_string");
```

```javascript
//results1 (serialized)

{
    "Input": "http://www.hostname.com:80",
    "Evaluated": "http://www.hostname.com:80",
    "Parsed": "www.hostname.com",
    "Type": DnsRip.InputType.Hostname
}

//results2 (serialized)

{
    "Input": "  http://192.168.10.1",
    "Evaluated": "http://192.168.10.1",
    "Parsed": "192.168.10.1",
    "Type": DnsRip.InputType.Ip
}

//results3 (serialized)

{
    "Input": "http://[FE80:0000:0000:0000:0202:B3FF:FE1E:8329]:8080/  ",
    "Evaluated": "http://[fe80:0000:0000:0000:0202:b3ff:fe1e:8329]:8080/",
    "Parsed": "fe80:0000:0000:0000:0202:b3ff:fe1e:8329",
    "Type": DnsRip.InputType.Ip
}

//results4 (serialized)

{
    "Input": "random_string",
    "Evaluated": "random_string",
    "Parsed": null,
    "Type": DnsRip.InputType.Invalid
}
```

## Credits
[Alphons van der Heijden](http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C). 
