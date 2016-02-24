using HCMUT.EMRCorefResol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.UITest
{
    class ConceptItem
    {
        public Concept Concept { get; }

        public int Index { get; }

        public bool IsType { get; }

        private readonly ConceptType _type;
        public virtual string Type
        {
            get { return $"t=\"{_type.ToString().ToLower()}\""; }
        }

        public ConceptItem(int index, Concept concept)
        {
            Concept = concept;
            Index = index;
            IsType = false;
        }

        public ConceptItem(int index, ConceptType type)
        {
            Index = index;
            _type = type;
            IsType = true;
        }
    }
}
