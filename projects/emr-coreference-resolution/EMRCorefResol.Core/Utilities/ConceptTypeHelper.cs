using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public static class ConceptTypeHelper
    {
        public static ConceptType Parse(string s, bool ignoreCase)
        {
            return (ConceptType)Enum.Parse(typeof(ConceptType), s, ignoreCase);
        }

        public static ConceptType Parse(string s)
        {
            return Parse(s, false);
        }
    }
}
