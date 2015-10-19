using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Xml;

namespace HCMUT.EMRCorefResol.English.Features
{
    class AliasFeature : Feature
    {
        public AliasFeature(IConceptPair instance)
            : base("Alias", 2, 0)
        {
            var ana = getAbbre(instance.Anaphora.Lexicon);
            var ante = getAbbre(instance.Antecedent.Lexicon);
            if (instance.Anaphora.Lexicon.Equals(ante, StringComparison.InvariantCultureIgnoreCase) ||
                instance.Antecedent.Lexicon.Equals(ana, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }

        private string getAbbre(string raw)
        {
            var arr = raw.ToLower().Split(' ');
            arr = arr.Select(i => i[0].ToString()).ToArray();

            return String.Join("", arr);
        }
    }
}
