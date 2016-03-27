using NUnit.Framework;
using System.Collections.Generic;

// ReSharper disable UnusedMethodReturnValue.Local

namespace DnsRip.Tests.Tests
{
    [TestFixture]
    public class ParserShould
    {
        public class ParseTest
        {
            public string Input { get; set; }
            public string Evaluated { get; set; }
            public string Parsed { get; set; }
            public DnsRip.InputType Type { get; set; }
        }

        private static IEnumerable<ParseTest> GetParseTests()
        {
            var tests = new List<ParseTest>
            {
                new ParseTest
                {
                    Input = "192.168.10.1",
                    Evaluated = "192.168.10.1",
                    Parsed = "192.168.10.1",
                    Type = DnsRip.InputType.Ip
                },
                new ParseTest
                {
                    Input = "http://192.168.10.1",
                    Evaluated = "http://192.168.10.1",
                    Parsed = "192.168.10.1",
                    Type = DnsRip.InputType.Ip
                },
                new ParseTest
                {
                    Input = "http://192.168.10.1/",
                    Evaluated = "http://192.168.10.1/",
                    Parsed = "192.168.10.1",
                    Type = DnsRip.InputType.Ip
                },
                new ParseTest
                {
                    Input = "random_string",
                    Evaluated = "random_string",
                    Parsed = null,
                    Type = DnsRip.InputType.Invalid
                },
                new ParseTest
                {
                    Input = " RANDOM_string ",
                    Evaluated = "random_string",
                    Parsed = null,
                    Type = DnsRip.InputType.Invalid
                },
                new ParseTest
                {
                    Input = "hostname.com",
                    Evaluated = "hostname.com",
                    Parsed = "hostname.com",
                    Type = DnsRip.InputType.Hostname
                },
                new ParseTest
                {
                    Input = "www.hostname.com",
                    Evaluated = "www.hostname.com",
                    Parsed = "www.hostname.com",
                    Type = DnsRip.InputType.Hostname
                },
                new ParseTest
                {
                    Input = "www.www.hostname.com",
                    Evaluated = "www.www.hostname.com",
                    Parsed = "www.www.hostname.com",
                    Type = DnsRip.InputType.Hostname
                },
                new ParseTest
                {
                    Input = "http://www.hostname.com",
                    Evaluated = "http://www.hostname.com",
                    Parsed = "www.hostname.com",
                    Type = DnsRip.InputType.Hostname
                },
                new ParseTest
                {
                    Input = "http://www.hostname.com/",
                    Evaluated = "http://www.hostname.com/",
                    Parsed = "www.hostname.com",
                    Type = DnsRip.InputType.Hostname
                },
                new ParseTest
                {
                    Input = "http://Hostname/  ",
                    Evaluated = "http://hostname/",
                    Parsed = null,
                    Type = DnsRip.InputType.Invalid
                }
            };

            foreach (var test in tests)
            {
                yield return test;
            }
        }

        [Test, TestCaseSource(nameof(GetParseTests))]
        public void ParseInput(ParseTest parseTest)
        {
            var dnsRip = new DnsRip.Parser(parseTest.Input);
            Assert.That(dnsRip.Evaluated, Is.EqualTo(parseTest.Evaluated));
            Assert.That(dnsRip.Parsed, Is.EqualTo(parseTest.Parsed));
            Assert.That(dnsRip.Type, Is.EqualTo(parseTest.Type));
        }

        //TODO: Cleanup

        //[Test]
        //public void HaveSubDomainsIfHostname()
        //{
        //    var dnsRip = new DnsRip("domain.com");
        //    Assert.That(dnsRip.SubDomains, Is.All.EndsWith("."));
        //}

        //[Test]
        //public void NotHaveSubDomainsIfIp()
        //{
        //    var dnsRip = new DnsRip("192.168.10.1");
        //    Assert.That(dnsRip.SubDomains, Is.Null);
        //}
    }
}