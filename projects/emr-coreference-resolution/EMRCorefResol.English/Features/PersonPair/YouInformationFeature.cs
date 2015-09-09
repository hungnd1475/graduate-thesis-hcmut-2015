using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class YouInformationFeature : Feature
    {
        public YouInformationFeature(PersonPair instance)
            : base("You-Information")
        {
            Value = (string.Equals(instance.Anaphora.Lexicon.ToLower(), "you") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "you")) ?
                1.0 : 0.0;
        }
    }
}
