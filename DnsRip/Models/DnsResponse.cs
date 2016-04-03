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

            Questions = new List<Question>();
            Answers = new List<AnswerRr>();

            for (var intI = 0; intI < header.QdCount; intI++)
            {
                Questions.Add(new Question(recordReader));
            }

            for (var intI = 0; intI < header.AnCount; intI++)
            {
                Answers.Add(new AnswerRr(recordReader));
            }
        }

        public List<Question> Questions;
        public List<AnswerRr> Answers;
    }
}