using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public static class EMRExtensions
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

        public static string GetLine(this EMR emr, Concept concept)
        {
            if(concept.Begin.Line == concept.End.Line)
            {
                return GetLine(emr, concept.Begin.Line).Replace("  ", " ").Replace("\r", "").Replace("\n", "");
            } else
            {
                var beginLine = GetLine(emr, concept.Begin.Line).Replace("  ", " ").Replace("\r", "").Replace("\n", "");
                var endLine = GetLine(emr, concept.End.Line).Replace("  ", " ").Replace("\r", "").Replace("\n", "");
                return beginLine + " " + endLine;
            }
        }

        public static string GetLine(this EMR emr, int lineNumber)
        {
            var lines = emr.Content.Split('\n');

            if (lineNumber > lines.Count() - 1)
            {
                return null;
            }
            return lines[lineNumber - 1];
        }

        public static EMRSection GetSection(this EMR emr, Concept c)
        {
            for(int i=0; i<emr.Sections.Count; i++)
            {
                var section = emr.Sections[i];
                if(section.Begin <= c.Begin.Line && section.End >= c.End.Line)
                {
                    return section;
                }
            }
            return null;
        }

        public static Concept GetPrevConcept(this EMR emr, Concept c)
        {
            var index = emr.Concepts.IndexOf(c);
            return (index > 0) ? emr.Concepts[index - 1] : null;
        }
    }
}
