using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PronounHeSheFeature : Feature
    {
        public PronounHeSheFeature(PersonInstance instance, int mostGender)
            : base("Pronoun-HeShe", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.HESHE_KEYWORDS;
            var indices = kw_searcher.SearchDictionaryIndices(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (indices.Length <= 0)
            {
                return;
            }

            if (mostGender == 0 && indices[0] <= 3)
            {
                SetCategoricalValue(1);
            }

            if (mostGender == 1 && indices[0] > 3)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
