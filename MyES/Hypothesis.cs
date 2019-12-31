using System;
using System.Collections.Generic;

namespace MyES
{
    class Hypothesis : ICloneable
    {
        public string Text { get; }
        public double PriorProbability { get; }
        public double Probability { get; set; }
        public Dictionary<int, Probability> PropertiesProbabilities { get; }

        public Hypothesis(string text, double initialProbability, Dictionary<int, Probability> probability)
        {
            Text = text;
            PriorProbability = initialProbability;
            Probability = initialProbability;
            PropertiesProbabilities = probability;
        }

        public object Clone()
        {
            return new Hypothesis(Text, PriorProbability, PropertiesProbabilities);
        }
    }
}
