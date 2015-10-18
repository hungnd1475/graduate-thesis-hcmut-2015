using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class StringMatchFeature : Feature
    {
        public StringMatchFeature(IConceptPair instance)
            : base("String-Match", 2, 0)
        {
            var searcher = new AhoCorasickKeywordDictionary("the", "a", "an", "my", "his", "her", "its", "their");
            var anaNorm = searcher.RemoveKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWordIngoreCase);
            var anteNorm = searcher.RemoveKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWordIngoreCase);

            if (string.Equals(anaNorm, anteNorm))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
