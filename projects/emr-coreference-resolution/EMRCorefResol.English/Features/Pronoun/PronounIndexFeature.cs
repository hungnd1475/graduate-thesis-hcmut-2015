using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PronounIndexFeature : Feature
    {
        public PronounIndexFeature(PronounInstance instance)
            : base("Pronoun-Index", 19, 0)
        {
            var searcher = KeywordService.Instance.PRONOUNS;
            var res = searcher.SearchDictionaryIndices(instance.Concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);
            if (res.Length > 0 && res[0] >= 0 && res[0] <= 17)
            {
                SetCategoricalValue(res[0] + 1);
            }
        }
    }
}
