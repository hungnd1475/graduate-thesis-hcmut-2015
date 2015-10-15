using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class IndicatorFeature : Feature
    {
        public IndicatorFeature(IConceptPair instance)
            :base("Indicator-Feature", 2, 0)
        {
            var searcher = KeywordService.Instance.INDICATOR_KEYWORD;

            var anaIndex = searcher.SearchDictionaryIndices(instance.Anaphora.Lexicon.Replace('-', ' '), KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);
            var anteIndex = searcher.SearchDictionaryIndices(instance.Antecedent.Lexicon.Replace('-', ' '), KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if(anaIndex.Length <=0 || anteIndex.Length <= 0)
            {
                return;
            }

            if(anaIndex.Intersect(anteIndex).Count() > 0)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
