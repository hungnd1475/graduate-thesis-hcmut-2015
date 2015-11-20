using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public static class ConceptTypeHelper
    {
        /// <summary>
        /// Parses a specified string represents a concept type to a value of <see cref="ConceptType"/> enum.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="ignoreCase">Indicates that if case should be ignored.</param>
        /// <returns></returns>
        public static ConceptType Parse(string s, bool ignoreCase)
        {
            return (ConceptType)Enum.Parse(typeof(ConceptType), s, ignoreCase);
        }

        /// <summary>
        /// Parses a specified string represents a concept type to a value of <see cref="ConceptType"/> enum,
        /// and ignores the case.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns></returns>
        public static ConceptType Parse(string s)
        {
            return Parse(s, false);
        }
    }
}
