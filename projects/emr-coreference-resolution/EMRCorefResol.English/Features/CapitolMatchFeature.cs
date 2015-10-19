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
            :base("Capitol-Match", 2, 0)
        {
            if (!checkPhrase(instance)) return;

            var anaAbbre = getAbbre(instance.Anaphora.Lexicon);
            var anteAbbre = getAbbre(instance.Antecedent.Lexicon);

            if(string.Equals(anaAbbre, anteAbbre, StringComparison.InvariantCultureIgnoreCase))
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
            var arr = raw.ToLower().Split(' ');
            arr = arr.Select(i => i[0].ToString()).ToArray();

            return String.Join("", arr);
        }
    }
}
