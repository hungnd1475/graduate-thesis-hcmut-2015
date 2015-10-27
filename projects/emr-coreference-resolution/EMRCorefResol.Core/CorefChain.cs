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
        private readonly HashSet<Concept> _conceptSet;

        /// <summary>
        /// Gets the coreference type of the chain.
        /// </summary>
        public ConceptType Type { get; }

        /// <summary>
        /// Gets the total number of concepts in chain.
        /// </summary>
        public int Count
        {
            get { return _conceptSet.Count; }
        }

        /// <summary>
        /// Initializes a <see cref="CorefChain"/> instance from a set of concepts.
        /// </summary>
        /// <param name="conceptSet">The set of concepts represents the chain.</param>
        /// <param name="antecedent">The antecedent of the chain.</param>
        /// <param name="type">The coreference type of the chain.</param>
        public CorefChain(HashSet<Concept> conceptSet, ConceptType type)
        {
            _conceptSet = conceptSet;
            Type = type;
        }

        /// <summary>
        /// Initializes a <see cref="CorefChain"/> instance from a sequence of concepts.
        /// </summary>
        /// <param name="conceptSet">The sequence of concepts represents the chain.</param>
        /// <param name="antecedent">The antecedent of the chain.</param>
        /// <param name="type">The coreference type of the chain.</param>
        public CorefChain(IEnumerable<Concept> conceptSet, ConceptType type)
            : this(new HashSet<Concept>(conceptSet), type)
        { }

        private CorefChain()
        {
            _conceptSet = new HashSet<Concept>();
            Type = ConceptType.None;
        }

        public bool Contains(Concept concept)
        {
            return _conceptSet.Contains(concept);
        }

        public CorefChain Intersect(CorefChain other)
        {
            CorefChain a = this, b = other;
            if (a.Count > b.Count)
            {
                GenericHelper.Swap(ref a, ref b);
            }

            var intersected = new HashSet<Concept>();
            foreach (var c in a)
            {
                if (b.Contains(c))
                {
                    intersected.Add(c);
                }
            }

            return new CorefChain(intersected, Type);
        }

        public CorefChain Except(CorefChain other)
        {
            return Except(other._conceptSet);
        }

        public CorefChain Except(HashSet<Concept> concepts)
        {
            var excepted = new HashSet<Concept>();
            foreach (var c in _conceptSet)
            {
                if (!concepts.Contains(c))
                {
                    excepted.Add(c);
                }
            }
            return new CorefChain(excepted, Type);
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
            return _conceptSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
