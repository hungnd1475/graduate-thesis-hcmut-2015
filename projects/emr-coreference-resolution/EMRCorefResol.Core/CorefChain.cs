using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class CorefChain : IReadOnlyCollection<Concept>
    {
        private readonly HashSet<Concept> _chain;

        public ConceptType Type { get; }

        public Concept Antecedent { get; }

        public int Count
        {
            get { return _chain.Count; }
        }

        public CorefChain(HashSet<Concept> chain, Concept antecedent, ConceptType type)
        {
            _chain = chain;
            Type = type;
            Antecedent = antecedent;
        }

        public CorefChain(IEnumerable<Concept> chain, Concept antecedent, ConceptType type)
            : this(new HashSet<Concept>(chain), antecedent, type)
        { }

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
