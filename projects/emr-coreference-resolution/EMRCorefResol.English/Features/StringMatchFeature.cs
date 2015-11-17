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
            var anaNorm = instance.Anaphora.Lexicon;
            var anteNorm = instance.Antecedent.Lexicon;

            if (instance.GetType().Name != "PersonPair")
            {
                anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon, KeywordService.Instance.STOP_WORDS);
                anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon, KeywordService.Instance.STOP_WORDS);
            }

            if (string.Equals(anaNorm, anteNorm))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
