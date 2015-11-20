using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class FalseFilterRule : IFilterRule
    {
        public bool IsSatisfied(IConceptPair pair)
        {
            return false;
        }

        public bool IsSatisfied(Concept antecedent, Concept anaphora)
        {
            return false;
        }
    }
}
