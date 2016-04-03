using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsRequest
    {
        public DnsRequest(DnsQuestion question, Header1 header)
        {
            _header = header;
            _question = question;
        }

        private readonly Header1 _header;
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