using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class FirstNextVerb : Feature
    {
        public FirstNextVerb(PronounInstance instance, EMR emr, IKeywordDictionary keywords)
            : base("FirstNext-Verb", keywords.Count + 1, 0)
        {
            var line = emr.GetLine(instance.Concept.End.Line);
            var tokens = line.Split(' ');
            var wi = instance.Concept.End.WordIndex + 1;

            if (wi >= 0 && wi < tokens.Length)
            {
                var nextWord = tokens[wi];
                nextWord = nextWord.Trim();

                var index = keywords.SearchDictionaryIndices(nextWord, KWSearchOptions.WholeWordIgnoreCase);
                if (index.Length > 0)
                {
                    SetCategoricalValue(index[0] + 1);
                }
            }
        }
    }
}
