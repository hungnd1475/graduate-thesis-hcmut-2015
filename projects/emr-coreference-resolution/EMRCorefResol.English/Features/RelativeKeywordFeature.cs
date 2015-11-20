using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class RelativeKeywordFeature : Feature
    {
        public RelativeKeywordFeature(ISingleConcept instance)
            : base("Relative-Keyword", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.RELATIVES;
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
        }

        public RelativeKeywordFeature(IConceptPair instance)
            :base("Relative-Keyword", 2, 0)
        {
            var searcher = KeywordService.Instance.RELATIVES;
            var key1 = searcher.SearchKeywords(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);
            var key2 = searcher.SearchKeywords(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if(key1.Intersect(key2).Count() > 0)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
