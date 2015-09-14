using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents the class label of concept appears in an EMR.
    /// Can also be used for coreference chains.
    /// </summary>
    public enum ConceptType
    {
        /// <summary>
        /// Represents type of concepts that have to type (i.e. from coref chain)
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents type of concepts that mention about person.
        /// </summary>
        Person,
        /// <summary>
        /// Represents type of concepts that mention about problem.
        /// </summary>
        Problem,
        /// <summary>
        /// Represents type of concepts that mention about treatment.
        /// </summary>
        Treatment,
        /// <summary>
        /// Represents type of concepts that mention about test.
        /// </summary>
        Test,
        /// <summary>
        /// Represents type of concepts that is pronoun.
        /// </summary>
        Pronoun
    }
}
