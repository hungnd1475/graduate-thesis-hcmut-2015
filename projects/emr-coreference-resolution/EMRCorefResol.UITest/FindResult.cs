using HCMUT.EMRCorefResol;
using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.UITest
{
    interface IFindResult : IEquatable<IFindResult>
    { 
        List<ConceptItem> ToConceptItems(int index);        
    }

    class ResultFromChain: IFindResult
    {
        public Concept Concept { get; }
        public IEnumerable<string> Terms { get; }
        public int ContainsIn { get; }

        public ResultFromChain(Concept concept, IEnumerable<string> terms, int containsIn)
        {
            Concept = concept;
            Terms = terms;
            ContainsIn = containsIn;
        }

        public override int GetHashCode()
        {
            return Concept.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResultFromChain);
        }

        public bool Equals(IFindResult other)
        {
            return Equals(other as ResultFromChain);
        }

        private bool Equals(ResultFromChain other)
        {
            return other?.Concept.Equals(Concept) ?? false;
        }

        public List<ConceptItem> ToConceptItems(int index)
        {
            var items = new List<ConceptItem>();
            items.Add(new ConceptItem(index, Concept));
            items.Add(new ConceptItem(index, Concept.Type));
            items.Add(new EmptyConcept(index, $"terms=\"{string.Join(",", Terms)}\""));
            items.Add(new EmptyConcept(index, $"chainIndex={ContainsIn}"));
            return items;
        }
    }

    class ResultFromSingletons : IFindResult
    {
        public Concept Concept1 { get; }
        public Concept Concept2 { get; }
        public IEnumerable<string> Terms { get; }

        public ResultFromSingletons(Concept concept1, Concept concept2, IEnumerable<string> terms)
        {
            Concept1 = concept1;
            Concept2 = concept2;
            Terms = terms;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Concept1.GetHashCode(), Concept2.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResultFromSingletons);
        }

        public bool Equals(IFindResult other)
        {
            return Equals(other as ResultFromSingletons);
        }

        private bool Equals(ResultFromSingletons other)
        {
            if (other != null)
            {
                return other.Concept1.Equals(Concept1) &&
                    other.Concept2.Equals(Concept2);
            }

            return false;
        }

        public List<ConceptItem> ToConceptItems(int index)
        {
            var items = new List<ConceptItem>();
            items.Add(new ConceptItem(index, Concept1));
            items.Add(new ConceptItem(index, Concept2));
            items.Add(new ConceptItem(index, Concept1.Type));
            items.Add(new EmptyConcept(index, $"terms=\"{string.Join(",", Terms)}\""));
            return items;
        }
    }
}
