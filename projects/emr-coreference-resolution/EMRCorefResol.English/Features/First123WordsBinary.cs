using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;

    class First123WordsBinary : Feature
    {
        public First123WordsBinary(ISingleConcept instance, EMR emr, bool before,
            IKeywordDictionary checker)
            : base("First-123-Words-" + (before ? "Before" : "After"), 2, 1)
        {
            var words = new HashSet<string>();
            var lineNumber = before ? instance.Concept.Begin.Line : instance.Concept.End.Line;
            var line = First123WordsBoW.RemoveNewLine(emr.GetLine(lineNumber)).Trim();
            var tokens = line.Split(' ');

            var step = before ? -1 : 1;
            var begin = before ? instance.Concept.Begin.WordIndex : instance.Concept.End.WordIndex;

            for (int i = 1; i < tokens.Length && words.Count < 3; i++)
            {
                var wi = begin + i * step;
                if (wi >= 0 && wi < tokens.Length)
                {
                    var w = tokens[wi].ToLower();
                    if (!First123WordsBoW.IsStopChar(w))
                        words.Add(w);
                }
                else
                {
                    break;
                }
            }

            if (words.Count > 0)
            {
                foreach (var w in words)
                {
                    if (!checker.Match(w, KWSearchOptions.WholeWordIgnoreCase))
                    {
                        SetCategoricalValue(0);
                        break;
                    }
                }
            }
            else
            {
                SetCategoricalValue(0);
            }
        }
    }
}
