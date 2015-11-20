using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class ContainFeature : Feature
    {
        public ContainFeature(IConceptPair instance)
            :base("Contain-Feature", 2, 0)
        {
            if (instance.Anaphora.Lexicon.Contains(instance.Antecedent.Lexicon) ||
                instance.Antecedent.Lexicon.Contains(instance.Anaphora.Lexicon))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
