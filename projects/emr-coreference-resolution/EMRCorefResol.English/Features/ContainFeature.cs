using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class ContainFeature : Feature
    {
        public ContainFeature(IConceptPair instance)
            :base("Contain-Feature", 2, 0)
        {
            var searcher = new AhoCorasickKeywordDictionary(new string[] { instance.Anaphora.Lexicon });
            if(searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
                return;
            }

            searcher = new AhoCorasickKeywordDictionary(new string[] { instance.Antecedent.Lexicon });
            if (searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
                return;
            }
        }
    }
}
