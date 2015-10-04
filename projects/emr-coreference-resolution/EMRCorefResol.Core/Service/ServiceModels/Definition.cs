using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class Definition
    {
        public WordType POS { get; }
        public string Gloss { get; }
        public string[] Words { get; }

        public Definition(WordType p, string g, string[] w)
        {
            POS = p;
            Gloss = g;
            Words = w;
        }

        public override string ToString()
        {
            return "(" + POS + "): " + Gloss;
        }
    }
}
