using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features.ProblemTreatmentTest
{
    using Utilities;
    class ArticleFeature : Feature
    {
        public ArticleFeature(IConceptPair instance)
            :base("Article-Feature", 0)
        {
            var anaIndex = GetArticleWordIndex(instance.Anaphora.Lexicon);
            var anteIndex = GetArticleWordIndex(instance.Antecedent.Lexicon);

            var pairIndex = (anaIndex - 1) * 3 + anteIndex;
            SetContinuousValue(pairIndex);
        }

        private int GetArticleWordIndex(string term)
        {
            var searcher = new AhoCorasickKeywordDictionary(new string[] { "a", "an" });
            if(searcher.Match(term, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                return 1;
            }

            searcher = new AhoCorasickKeywordDictionary(new string[] { "the", "his", "her", "my", "their", "that" });
            if (searcher.Match(term, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                return 2;
            }

            return 3;
        }
    }
}
