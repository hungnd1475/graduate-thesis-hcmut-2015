using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public struct TemporalKey
    {
        public Concept Concept { get; }
        public EMR EMR { get; }

        public TemporalKey(Concept concept, EMR emr)
        {
            Concept = concept;
            EMR = emr;
        }
    }
}
