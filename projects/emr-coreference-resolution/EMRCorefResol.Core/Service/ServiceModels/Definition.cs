using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class Definition
    {
        public WordType POS { get; set; }
        public string Gloss { get; set; }
        public string[] Words { get; set; }

        /*public Definition(WordType p, string g, string[] w)
        {
            POS = p;
            Gloss = g;
            Words = w;
        }*/

        /*public Definition(int p, string g, string[] w)
        {
            switch (p)
            {
                case 0:
                    POS = WordType.None;
                    break;
                case 1:
                    POS = WordType.Noun;
                    break;
                case 2:
                    POS = WordType.Verb;
                    break;
                case 3:
                    POS = WordType.Adjective;
                    break;
                case 4:
                    POS = WordType.Adverb;
                    break;
                default:
                    POS = WordType.Noun;
                    break;
            }

            Gloss = g;
            Words = w;
        }*/

        public override string ToString()
        {
            return "(" + POS + "): " + Gloss;
        }
    }
}
