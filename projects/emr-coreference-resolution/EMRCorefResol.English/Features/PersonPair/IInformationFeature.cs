using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class IInformationFeature : Feature
    {
        public IInformationFeature(PersonPair instance)
            : base("I-Information", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.I_KEYWORDS;
            var anaIsI = kw_searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            var anteIsI = kw_searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (anaIsI && anteIsI)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
