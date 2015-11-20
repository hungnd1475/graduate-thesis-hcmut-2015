using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class YouInformationFeature : Feature
    {
        public YouInformationFeature(PersonPair instance)
            : base("You-Information", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.YOU_KEYWORDS;
            var anaIsI = kw_searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            var anteIsI = kw_searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (anaIsI && anteIsI)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
