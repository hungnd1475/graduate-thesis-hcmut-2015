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
            : base("I-Information", new[] { 1d, 0d })
        {
            if (string.Equals(instance.Anaphora.Lexicon.ToLower(), "i") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "i"))
            {
                Value[0] = 0d;
                Value[1] = 1d;
            }
        }
    }
}
