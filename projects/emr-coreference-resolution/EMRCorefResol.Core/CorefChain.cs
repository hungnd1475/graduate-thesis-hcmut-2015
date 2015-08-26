using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a coreference chain.
    /// </summary>
    public class CorefChain : IReadOnlyCollection<Concept>
    {
        private readonly HashSet<Concept> _chain;

        /// <summary>
        /// Gets the coreference type of the chain.
        /// </summary>
        public ConceptType Type { get; }

        /// <summary>
        /// Gets the antecedent of the chain, i.e. the concept in chain that appears first in the EMR.
        /// </summary>
        public Concept Antecedent { get; }

        /// <summary>
        /// Gets the total number of concepts in chain.
        /// </summary>
        public int Count
        {
            get { return _chain.Count; }
        }

        /// <summary>
        /// Initializes a <see cref="CorefChain"/> instance from a set of concepts.
        /// </summary>
        /// <param name="chain">The set of concepts represents the chain.</param>
        /// <param name="antecedent">The antecedent of the chain.</param>
        /// <param name="type">The coreference type of the chain.</param>
        public CorefChain(HashSet<Concept> chain, Concept antecedent, ConceptType type)
        {
            _chain = chain;
            Type = type;
            Antecedent = antecedent;
        }

        /// <summary>
        /// Initializes a <see cref="CorefChain"/> instance from a sequence of concepts.
        /// </summary>
        /// <param name="chain">The sequence of concepts represents the chain.</param>
        /// <param name="antecedent">The antecedent of the chain.</param>
        /// <param name="type">The coreference type of the chain.</param>
        public CorefChain(IEnumerable<Concept> chain, Concept antecedent, ConceptType type)
            : this(new HashSet<Concept>(chain), antecedent, type)
        { }

        public bool Contains(Concept concept)
        {
            return _chain.Contains(concept);
        }

        /// <summary>
        /// Gets the string represents the chain.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("||", this.Select(c => $"{c}")) 
                + $"||t=\"coref {Type.ToString().ToLower()}\"";
        }

        public IEnumerator<Concept> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
