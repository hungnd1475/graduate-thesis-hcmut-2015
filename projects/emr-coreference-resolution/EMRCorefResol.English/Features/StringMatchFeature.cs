using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class StringMatchFeature : Feature
    {
        public StringMatchFeature(IConceptPair instance)
            : base("String-Match")
        {
            Value = string.Equals(instance.Anaphora.Lexicon, instance.Antecedent.Lexicon) ?
                1.0 : 0.0;
        }
    }
}
