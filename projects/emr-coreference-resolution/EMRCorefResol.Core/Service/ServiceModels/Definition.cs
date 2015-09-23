using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class Definition
    {
        public WordType pos;
        public string Gloss;
        public string[] Words;
        public Definition(WordType p, string g, string[] w)
        {
            pos = p;
            Gloss = g;
            Words = w;
        }
        public override string ToString()
        {
            return "(" + pos + "): " + Gloss;
        }
    }
}
