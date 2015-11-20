using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class WeInformationFeature : Feature
    {
        public WeInformationFeature(PersonPair instance)
            : base("We-Information", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.WE_KEYWORDS;
            var anaIsWe = kw_searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            var anteIsWe = kw_searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (anaIsWe && anteIsWe)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
