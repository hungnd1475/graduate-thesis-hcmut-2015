using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    public class SubstringMatchFilterRule : IFilterRule
    {
        public bool IsSatisfied(IConceptPair pair)
        {
            var sm = new SubstringFeature(pair);
            return sm.GetCategoricalValue() == 1;
        }

        public bool IsSatisfied(Concept antecedent, Concept anaphora)
        {
            return IsSatisfied(PairInstance.Create(antecedent, anaphora));
        }
    }
}
