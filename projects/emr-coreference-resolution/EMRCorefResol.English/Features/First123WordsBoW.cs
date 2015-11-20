using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;

    class First123WordsBoW : Feature
    {
        public First123WordsBoW(ISingleConcept instance, EMR emr, bool before, IKeywordDictionary keywords)
            : base("First-123-Words-" + (before ? "Before" : "After"), keywords.Count)
        {
            var words = new HashSet<string>();
            var lineNumber = before ? instance.Concept.Begin.Line : instance.Concept.End.Line;
            var line = RemoveNewLine(emr.GetLine(lineNumber)).Trim();
            var tokens = line.Split(' ');

            var step = before ? -1 : 1;
            var begin = before ? instance.Concept.Begin.WordIndex : instance.Concept.End.WordIndex;

            for (int i = 1; i < tokens.Length && words.Count < 3; i++)
            {
                var wi = begin + i * step;
                if (wi >= 0 && wi < tokens.Length)
                {
                    var w = tokens[wi].ToLower();
                    if (!IsStopChar(w))
                        words.Add(w);
                }
                else
                {
                    break;
                }
            }

            Value[0] = 0d;
            foreach (var w in words)
            {
                var indices = keywords.SearchDictionaryIndices(w, KWSearchOptions.WholeWordIgnoreCase);
                foreach (var i in indices)
                {
                    Value[i] = 1d;
                }
            }
        }

        public static bool IsStopChar(string s)
        {
            var stopChars = new HashSet<string>()
            {
                ",", ";", ".", ":", "-", "*", "(", ")",
                "[", "]", "!", "?", ">", "<", "\"", "'",
                "{", "}", "\\", "|", "&", "^", "%", "$",
                "#", "@", "_", "+", "=", "~", "`", "/"
            };

            return s == string.Empty || stopChars.Contains(s);
        }

        public static string RemoveNewLine(string s)
        {
            return s.Replace(Environment.NewLine, " ");
        }
    }
}
