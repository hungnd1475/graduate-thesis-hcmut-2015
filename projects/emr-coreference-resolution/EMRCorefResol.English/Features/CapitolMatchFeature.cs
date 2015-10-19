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
            var alias = new AliasFeature(instance);
            if (alias.GetCategoricalValue() == 1)
            {
                SetCategoricalValue(1);
                return;
            }

            var anaAbbre = getAbbre(instance.Anaphora.Lexicon);
            var anteAbbre = getAbbre(instance.Antecedent.Lexicon);

            if (string.Equals(anaAbbre, anteAbbre, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
                return;
            }
        }

        private string getAbbre(string raw)
        {
            var arr = raw.Split(' ');
            return arr.Select(i => i.Length > 0 ? i[0].ToString() : string.Empty)
                .Aggregate(string.Empty, (s, c) => s + c);
        }
    }
}
