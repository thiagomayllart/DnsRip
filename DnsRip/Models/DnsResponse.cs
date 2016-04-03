using System;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsResponse
    {
        public DnsResponse(byte[] data)
        {
            var recordReader = new RecordReader(data);
            var header = new Header1(recordReader);

            Questions = new List<DnsQuestion>();
            Answers = new List<AnswerRr>();

            for (var intI = 0; intI < header.QdCount; intI++)
            {
                Questions.Add(new DnsQuestion(recordReader));
            }

            for (var intI = 0; intI < header.AnCount; intI++)
            {
                Answers.Add(new AnswerRr(recordReader));
            }
        }

        public List<DnsQuestion> Questions;
        public List<AnswerRr> Answers;
    }
}