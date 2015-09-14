using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SemanticFeature : Feature
    {
        public SemanticFeature(PronounInstance instance, EMR emr)
            : base("Semantic-Feature")
        {
            var line = emr.getLine(instance.Concept.Begin.Line);

            Value = 0.0;
        }
    }
}
