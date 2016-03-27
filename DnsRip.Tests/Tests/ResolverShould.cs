using DnsRip.Interfaces;
using DnsRip.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
            public bool? RecordIsIp { get; set; }
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
                            RecordIsIp = true
                        }
                    }
                },
                new ResolveTest
                {
                    Query = "fd-fp3.wg1.b.yahoo.com",
                    Type = DnsRip.QueryType.A,
                    IsRecursive = true,
                    Expected = new List<TestResponse>
                    {
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp = true
                        },
                        new TestResponse
                        {
                            Host = "fd-fp3.wg1.b.yahoo.com.",
                            Type = DnsRip.QueryType.A,
                            RecordIsIp = true
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
                var expected = (TestResponse)expectedSet[index];

                Assert.That(result.Host, Is.EqualTo(expected.Host));
                Assert.That(result.Type, Is.EqualTo(expected.Type));
                Assert.That(DnsRip.Utilities.IsInteger(result.Ttl), Is.EqualTo(expected.TtlIsInteger));

                if (expected.RecordIsIp.HasValue)
                    Assert.That(DnsRip.Utilities.IsIp(result.Record), Is.EqualTo(expected.RecordIsIp));

                if (expected.RecordIsHostname.HasValue)
                    Assert.That(DnsRip.Utilities.IsHostname(result.Record), Is.EqualTo(expected.RecordIsHostname));

                index++;
            }
        }
    }
}