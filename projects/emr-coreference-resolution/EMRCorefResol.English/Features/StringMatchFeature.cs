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
            var searcher = new AhoCorasickKeywordDictionary(new string[] { "the", "a", "an" });
            var anaNorm = searcher.RemoveKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            var anteNorm = searcher.RemoveKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (string.Equals(anaNorm, anteNorm))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
