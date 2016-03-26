using NUnit.Framework;
using System.Collections.Generic;

// ReSharper disable UnusedMethodReturnValue.Local

namespace DnsRip.Tests
{
    [TestFixture]
    public class DnsRipShould
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
            var parseTests = new List<ParseTest>
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

            foreach (var parseTest in parseTests)
            {
                yield return parseTest;
            }
        }

        [Test, TestCaseSource(nameof(GetParseTests))]
        public void ParseInput(ParseTest parseTest)
        {
            var dnsRip = new DnsRip(parseTest.Input);
            Assert.That(dnsRip.Evaluated, Is.EqualTo(parseTest.Evaluated));
            Assert.That(dnsRip.Parsed, Is.EqualTo(parseTest.Parsed));
            Assert.That(dnsRip.Type, Is.EqualTo(parseTest.Type));
        }
    }
}