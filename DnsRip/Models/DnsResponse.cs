using System;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsResponse
    {
        public DnsResponse(byte[] data)
        {
            var recordReader = new RecordReader(data);
            var header = new DnsHeader(recordReader);

            Questions = new List<DnsQuestion>();
            Answers = new List<AnswerRr>();
            Authorities = new List<AuthorityRr>();
            Additionals = new List<AdditionalRr>();

            for (var intI = 0; intI < header.QdCount; intI++)
            {
                Questions.Add(new DnsQuestion(recordReader));
            }

            for (var intI = 0; intI < header.AnCount; intI++)
            {
                Answers.Add(new AnswerRr(recordReader));
            }

            for (var intI = 0; intI < header.NsCount; intI++)
            {
                Authorities.Add(new AuthorityRr(recordReader));
            }

            for (var intI = 0; intI < header.ArCount; intI++)
            {
                Additionals.Add(new AdditionalRr(recordReader));
            }
        }

        public List<DnsQuestion> Questions;
        public List<AnswerRr> Answers;
        public List<AuthorityRr> Authorities;
        public List<AdditionalRr> Additionals;
    }
}