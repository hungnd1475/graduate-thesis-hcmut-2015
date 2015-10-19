using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a coreference chain.
    /// </summary>
    public class CorefChain : IReadOnlyCollection<Concept>
    {
        public static readonly CorefChain Empty = new CorefChain();

        private readonly HashSet<Concept> _chain;

        /// <summary>
        /// Gets the coreference type of the chain.
        /// </summary>
        public ConceptType Type { get; }

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
        public CorefChain(HashSet<Concept> chain, ConceptType type)
        {
            _chain = chain;
            Type = type;
        }

        /// <summary>
        /// Initializes a <see cref="CorefChain"/> instance from a sequence of concepts.
        /// </summary>
        /// <param name="chain">The sequence of concepts represents the chain.</param>
        /// <param name="antecedent">The antecedent of the chain.</param>
        /// <param name="type">The coreference type of the chain.</param>
        public CorefChain(IEnumerable<Concept> chain, ConceptType type)
            : this(new HashSet<Concept>(chain), type)
        { }

        private CorefChain()
        {
            _chain = new HashSet<Concept>();
            Type = ConceptType.None;
        }

        public bool Contains(Concept concept)
        {
            return _chain.Contains(concept);
        }

        public CorefChain Intersect(CorefChain other)
        {
            CorefChain a = this, b = other;
            if (a.Count > b.Count)
            {
                GenericHelper.Swap(ref a, ref b);
            }

            var concepts = new HashSet<Concept>();
            foreach (var c in a)
            {
                if (b.Contains(c))
                {
                    concepts.Add(c);
                }
            }

            return new CorefChain(concepts, ConceptType.None);
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
