using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyES
{
    class MkbFile : ICloneable
    {
        public List<string> Info { get; }
        public List<Question> Questions { get; }
        public List<Hypothesis> Hypotheses { get; }

        public MkbFile(List<string> info, List<Question> questions, List<Hypothesis> answers)
        {
            Info = info;
            Questions = questions;
            Hypotheses = answers;
        }

        public string InfoToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var line in Info)
            {
                stringBuilder.Append(line + "\n");
            }
            return stringBuilder.ToString();
        }

        public object Clone()
        {
            List<Question> questionsClone = Questions.Select(item => (Question)item.Clone()).ToList();
            List<Hypothesis> hypothesesClone = Hypotheses.Select(item => (Hypothesis)item.Clone()).ToList();

            return new MkbFile(new List<string>(Info), questionsClone, hypothesesClone);
        }
    }
}
