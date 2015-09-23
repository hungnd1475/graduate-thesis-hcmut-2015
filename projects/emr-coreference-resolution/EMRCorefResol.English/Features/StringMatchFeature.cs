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
            : base("String-Match", new[] { 1d, 0d })
        {
            if (string.Equals(instance.Anaphora.Lexicon, instance.Antecedent.Lexicon))
            {
                Value[0] = 0d;
                Value[1] = 1d;
            }
        }
    }
}
