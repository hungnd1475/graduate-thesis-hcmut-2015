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
            : base("String-Match", 2, 0)
        {
            if (string.Equals(instance.Anaphora.Lexicon, instance.Antecedent.Lexicon))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
