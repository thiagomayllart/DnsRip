using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsRequest
    {
        public DnsRequest(DnsQuestion question, DnsHeader header)
        {
            _header = header;
            _question = question;
        }

        private readonly DnsHeader _header;
        private readonly DnsQuestion _question;

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(_header.Data);
                data.AddRange(_question.Data);
                return data.ToArray();
            }
        }
    }
}