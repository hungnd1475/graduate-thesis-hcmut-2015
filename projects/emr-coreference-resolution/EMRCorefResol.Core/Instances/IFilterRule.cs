using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFilterRule
    {
        bool IsSatisfied(Concept antecedent, Concept anaphora);
        bool IsSatisfied(IConceptPair pair);
    }
}
