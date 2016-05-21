using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DnsRip.Tests
{
    [TestFixture]
    public class TypeTest
    {
        [Test]
        public void Validate()
        {
            var input = "A";

            QueryType type;
            QueryType? parse = null;

            var result = Enum.TryParse(input, out type);
            
            if (result)
                parse = (QueryType)Enum.Parse(typeof (QueryType), input);

            Assert.That(result, Is.True);
            Assert.That(type, Is.EqualTo(QueryType.A));
            Assert.That(parse, Is.EqualTo(QueryType.A));
        }
    }
}
