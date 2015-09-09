using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class IInformationFeature : Feature
    {
        public IInformationFeature(PersonPair instance)
            : base("I-Information")
        {
            Value = (string.Equals(instance.Anaphora.Lexicon.ToLower(), "i") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "i")) ?
                1.0 : 0.0;
        }
    }
}
