using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class EquipmentFeature : Feature
    {
        public EquipmentFeature(IConceptPair instance)
            :base("Equipment-Feature", 3, 2)
        {
            var anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon);
            var anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon);

            var anaUMLS = Service.English.GetUMLSInformation(anaNorm, Service.UMLSUtil.UMLS_EQUIPMENT);
            var anteUMLS = Service.English.GetUMLSInformation(anteNorm, Service.UMLSUtil.UMLS_EQUIPMENT);

            if (anaUMLS == null || anteUMLS == null)
            {
                return;
            }
            else
            {
                //If concept contain keyword
                var searcher = new AhoCorasickKeywordDictionary(new string[] { "graphy", "gram", "metry", "scopy" });
                if(!searcher.Match(anaNorm, KWSearchOptions.IgnoreCase) || !searcher.Match(anteNorm, KWSearchOptions.IgnoreCase))
                {
                    return;
                }
                else if (anaUMLS.Concept.Equals(anteNorm, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Concept, StringComparison.InvariantCultureIgnoreCase) ||
                    anaUMLS.Concept.Equals(anteUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
                    anteUMLS.Concept.Equals(anaUMLS.Prefer, StringComparison.InvariantCultureIgnoreCase) ||
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
