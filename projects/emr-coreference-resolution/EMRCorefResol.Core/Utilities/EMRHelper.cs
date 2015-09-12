using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public static class EMRHelper
    {
        public static int BeginIndexOf(this EMR emr, Concept c)
        {
            int line = 1, index = 0, nextIndex = 0;
            while (line < c.Begin.Line)
            {
                index = nextIndex;
                nextIndex = emr.Content.IndexOf(Environment.NewLine, nextIndex) + Environment.NewLine.Length;
                line += 1;
            }

            int word = -1;
            while (word < c.Begin.WordIndex)
            {
                index = nextIndex;
                nextIndex = emr.Content.IndexOf(' ', nextIndex) + 1;
                word += 1;
            }

            return index;
        }

        public static int EndIndexOf(this EMR emr, Concept c)
        {
            var bIndex = emr.BeginIndexOf(c);
            return bIndex + c.Lexicon.Length - 1;
        }

        public static string ContentBetween(this EMR emr, Concept c1, Concept c2)
        {
            var begin = emr.EndIndexOf(c1);
            var end = emr.BeginIndexOf(c2);
            var length = end - begin - 1;
            return length > 0 ? emr.Content.Substring(begin + 1, length)
                : string.Empty;
        }

        public static string ContentBetween(this EMR emr, IConceptPair pair)
        {
            return emr.ContentBetween(pair.Antecedent, pair.Anaphora);
        }
    }
}
