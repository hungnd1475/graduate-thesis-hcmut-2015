using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class OperationFeature : Feature
    {
        public OperationFeature(IConceptPair instance, UMLSDataDictionary dictionary)
            :base("Operation-Feature",3, 2)
        {
            var anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon, KeywordService.Instance.STOP_WORDS);
            var anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon, KeywordService.Instance.STOP_WORDS);

            var anaUMLS = dictionary.Get(instance.Anaphora.Lexicon + "|OPE");
            var anteUMLS = dictionary.Get(instance.Antecedent.Lexicon + "|OPE");

            if (anaUMLS == null || anteUMLS == null)
            {
                return;
            }
            else
            {
                //If concept contain keyword
                var searcher = new AhoCorasickKeywordDictionary(new string[] { "tomy", "plasty" });
                if (!searcher.Match(anaNorm, KWSearchOptions.IgnoreCase) || !searcher.Match(anteNorm, KWSearchOptions.IgnoreCase))
                {
                    return;
                }
                if (anaUMLS.Concept.Equals(anteNorm, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Concept, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
                    anteUMLS.Concept.Equals(anaUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Prefer.Equals(anteUMLS.Prefer, StringComparison.InvariantCulture) ||
                    anteUMLS.Concept.Equals(anaNorm))
                {
                    SetCategoricalValue(1);
                }
                else
                {
                    SetCategoricalValue(0);
                }
            }
        }
    }
}
