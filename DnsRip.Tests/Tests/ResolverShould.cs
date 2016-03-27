using DnsRip.Interfaces;
using DnsRip.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// ReSharper disable UnusedMethodReturnValue.Local

namespace DnsRip.Tests.Tests
{
    [TestFixture]
    public class ResolverShould
    {
        public class ResolveTest : IDnsRipRequest
        {
            public string Query { get; set; }
            public DnsRip.QueryType Type { get; set; }
            public bool IsRecursive { get; set; }
            public IEnumerable<DnsRipResponse> Expected { get; set; }

            public IEnumerable<string> Servers
            {
                get { return new[] { "8.8.4.4" }; }
                set { }
            }
        }

        public class TestResponse : DnsRipResponse
        {
            public bool TtlIsInteger => true;
            public bool? RecordIsIp4 { get; set; }
            public bool? RecordIsIp6 { get; set; }
            public bool? RecordIsHostname { get; set; }
        }

        private static IEnumerable<ResolveTest> GetResolveTests()
        {
            var tests = new List<ResolveTest>
            {
                new ResolveTest
                {
                    Query = "google.com",
                    Type = DnsRip.QueryType.A,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        }
                    }
                },
                new ResolveTest
                {
                    Query = "yahoo.com",
                    Type = DnsRip.QueryType.A,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        },
                        new TestResponse
                        {
                            Host = "yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        },
                        new TestResponse
                        {
                            Host = "yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        }
                    }
                },
                new ResolveTest
                {
                    Query = "www.yahoo.com",
                    Type = DnsRip.QueryType.A,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "www.yahoo.com.",
                            Type = DnsRip.QueryType.CNAME,
                            RecordIsHostname = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp4 = true
                        }
                    }
                },
                new ResolveTest
                {
                    Query = "google.com",
                    Type = DnsRip.QueryType.AAAA,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.AAAA,
                            RecordIsIp6 = true
                        }
                    }
                },
                new ResolveTest
                {
                    Query = "google.com",
                    Type = DnsRip.QueryType.NS,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.NS,
                            RecordIsHostname = true
                        },
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.NS,
                            RecordIsHostname = true
                        },
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.NS,
                            RecordIsHostname = true
                        },
                        new TestResponse
                        {
                            Host = "google.com.",
                            Type = DnsRip.QueryType.NS,
                            RecordIsHostname = true
                        }
                    }
                }
            };

            foreach (var test in tests)
            {
                yield return test;
            }
        }

        [Test, TestCaseSource(nameof(GetResolveTests))]
        public void Resolve(ResolveTest resolveTest)
        {
            var dnsRip = new DnsRip.Resolver(resolveTest);
            var resultSet = dnsRip.Resolve();
            var expectedSet = resolveTest.Expected.ToList();
            var index = 0;

            foreach (var result in resultSet)
            {
                Console.WriteLine(result.Host);
                Console.WriteLine(result.Ttl);
                Console.WriteLine(result.Type);
                Console.WriteLine(result.Record);
                Console.WriteLine();

                var expected = (TestResponse)expectedSet[index];

                Assert.That(result.Host, Is.EqualTo(expected.Host));
                Assert.That(result.Type, Is.EqualTo(expected.Type));
                Assert.That(DnsRip.Utilities.IsInteger(result.Ttl), Is.EqualTo(expected.TtlIsInteger));

                if (expected.RecordIsIp4.HasValue)
                    Assert.That(Uri.CheckHostName(result.Record) == UriHostNameType.IPv4, Is.EqualTo(expected.RecordIsIp4));

                if (expected.RecordIsIp6.HasValue)
                    Assert.That(Uri.CheckHostName(result.Record) == UriHostNameType.IPv6, Is.EqualTo(expected.RecordIsIp6));

                if (expected.RecordIsHostname.HasValue)
                    Assert.That(Uri.CheckHostName(result.Record) == UriHostNameType.Dns, Is.EqualTo(expected.RecordIsHostname));

                index++;
            }
        }
    }
}