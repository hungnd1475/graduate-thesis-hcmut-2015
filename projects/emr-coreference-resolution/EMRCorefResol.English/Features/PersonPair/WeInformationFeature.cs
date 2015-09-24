using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WeInformationFeature : Feature
    {
        public WeInformationFeature(PersonPair instance)
            : base("We-Information", 2, 0)
        {
            if ((string.Equals(instance.Anaphora.Lexicon.ToLower(), "we") &&
                string.Equals(instance.Antecedent.Lexicon.ToLower(), "we")))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
