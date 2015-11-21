using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class AnatomyFeature : Feature
    {
        public AnatomyFeature(IConceptPair instance, UMLSDataDictionary dictionary)
            : base("Anatomy-Feature", 3, 2)
        {
            var anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon, KeywordService.Instance.STOP_WORDS);
            var anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon, KeywordService.Instance.STOP_WORDS);

            var anaUMLS = dictionary.Get(instance.Anaphora.Lexicon + "|ANA");
            var anteUMLS = dictionary.Get(instance.Antecedent.Lexicon + "|ANA");

            if (anaUMLS == null || anteUMLS == null)
            {
                return;
            }
            else
            {
                if (anaUMLS.Concept.Equals(anteNorm, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Concept, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
                    anteUMLS.Concept.Equals(anaUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Prefer.Equals(anteUMLS.Prefer, StringComparison.InvariantCulture) ||
                    anteUMLS.Concept.Equals(anaNorm, StringComparison.InvariantCultureIgnoreCase))
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
