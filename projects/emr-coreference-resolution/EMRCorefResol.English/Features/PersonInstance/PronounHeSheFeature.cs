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
        public PronounHeSheFeature(PersonInstance instance)
            :base("Pronoun-HeShe", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.HESHE_KEYWORDS;
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
