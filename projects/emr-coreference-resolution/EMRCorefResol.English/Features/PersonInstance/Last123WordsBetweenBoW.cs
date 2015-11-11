using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;

    class Last123WordsBetweenBoW : Feature
    {
        public Last123WordsBetweenBoW(PersonInstance instance, EMR emr, ConceptType c2Type,
            IKeywordDictionary keywords)
            : base("Last-123-Words-Between-" + c2Type.ToString(), keywords.Count, 0)
        {
            var c1 = instance.Concept;
            var c1Index = emr.Concepts.IndexOf(c1);
            var words = new HashSet<string>();

            for (int i = c1Index + 1; i < emr.Concepts.Count; i++)
            {
                var c2 = emr.Concepts[i];
                if (c2.Type == c2Type)
                {
                    var s = First123WordsBoW.RemoveNewLine(emr.ContentBetween(c1, c2)).Trim();
                    var tokens = s.Split(' ');

                    for (int j = tokens.Length - 1; j >= 0 && words.Count < 3; j--)
                    {
                        var w = tokens[j].ToLower();
                        if (!First123WordsBoW.IsStopChar(w))
                            words.Add(w);
                    }

                    break;
                }
            }

            Value[0] = 0;
            foreach (var w in words)
            {
                var indices = keywords.SearchDictionaryIndices(w, KWSearchOptions.WholeWordIgnoreCase);
                foreach (var i in indices)
                {
                    Value[i] = 1d;
                }
            }
        }
    }
}
