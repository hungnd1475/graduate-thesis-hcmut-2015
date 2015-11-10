using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class CapitolMatchFeature : Feature
    {
        public CapitolMatchFeature(IConceptPair instance)
            : base("Capitol-Match", 2, 0)
        {
            if (!checkPhrase(instance)) return;

            var anaNormalized = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon);
            var anteNormalized = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon);

            var anaAbbre = getAbbre(anaNormalized);
            var anteAbbre = getAbbre(anteNormalized);

            if (string.Equals(anaAbbre, anteAbbre, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
                return;
            }
        }

        private bool checkPhrase(IConceptPair pair)
        {
            var anaArr = pair.Anaphora.Lexicon.Split(' ');
            var anteArr = pair.Antecedent.Lexicon.Split(' ');

            return (anaArr.Length == 1 && anteArr.Length == 1) ? false : true;
        }

        private string getAbbre(string raw)
        {
            var arr = raw.Split(' ');
            return arr.Select(i => i.Length > 0 ? i[0].ToString() : string.Empty)
                .Aggregate(string.Empty, (s, c) => s + c);
        }
    }
}
