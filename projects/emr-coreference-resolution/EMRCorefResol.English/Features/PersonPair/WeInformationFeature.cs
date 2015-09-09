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
            : base("We-Information")
        {
            Value = (string.Equals(instance.Anaphora.Lexicon.ToLower(), "we") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "we")) ?
                1.0 : 0.0;
        }
    }
}
