using HCMUT.EMRCorefResol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.UITest
{
    class EmptyConcept : ConceptItem
    {
        private readonly string _message;
        public override string Type
        {
            get { return _message; }
        }

        public EmptyConcept(string message)
            : base(1, ConceptType.None)
        {
            _message = message;
        }

        public EmptyConcept(int index, string message)
            : base(index, ConceptType.None)
        {
            _message = message;
        }
    }
}
