using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public struct MedKey
    {
        public Concept Concept { get; }
        public EMR EMR { get; }

        public MedKey(Concept concept, EMR emr)
        {
            Concept = concept;
            EMR = emr;
        }
    }
}
