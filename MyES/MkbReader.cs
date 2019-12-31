using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MyES
{
    class MkbReader
    {
        public static MkbFile ReadFile(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.GetEncoding("windows-1251")))
            {
                List<string> info = ReadToEmptyLine(streamReader);

                streamReader.ReadLine();
                List<Question> questions = new List<Question>();
                foreach(var line in ReadToEmptyLine(streamReader))
                {
                    questions.Add(new Question(line));
                }
                
                List<Hypothesis> answers = ParceAnswers(ReadToEmptyLine(streamReader));

                return new MkbFile(info, questions, answers);
            }
        }

        private static List<string> ReadToEmptyLine(StreamReader streamReader)
        {
            List<string> listString = new List<string>();
            string line;
            while (!string.IsNullOrEmpty((line = streamReader.ReadLine())))
            {
                listString.Add(line);
            }
            return listString;
        }

        private static List<Hypothesis> ParceAnswers(List<string> answers)
        {
            List<Hypothesis> parcedAnswers = new List<Hypothesis>();
            foreach (var answer in answers)
            {
                string[] strings = answer.Split(',');
                Dictionary<int, Probability> probabilities = new Dictionary<int, Probability>();

                for (int i = 2; i < strings.Length; i++)
                {
                    probabilities.Add(
                        int.Parse(strings[i]),
                        new Probability(
                            double.Parse(strings[++i], CultureInfo.CreateSpecificCulture("en-US")),
                            double.Parse(strings[++i], CultureInfo.CreateSpecificCulture("en-US")))
                        );
                }
                parcedAnswers.Add(new Hypothesis(strings[0], double.Parse(strings[1], CultureInfo.CreateSpecificCulture("en-US")), probabilities));
            }
            return parcedAnswers;
        }
    }
}
