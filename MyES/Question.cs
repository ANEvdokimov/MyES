using System;

namespace MyES
{
    class Question : ICloneable
    {
        public string Text { get; }
        public int Answer { get; set; }

        public Question(string text)
        {
            Text = text;
        }

        public object Clone()
        {
            return new Question(Text);
        }
    }
}
