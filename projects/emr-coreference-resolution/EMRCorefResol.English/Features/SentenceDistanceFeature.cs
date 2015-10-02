using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SentenceDistanceFeature : Feature
    {
        public SentenceDistanceFeature(IConceptPair instance, EMR emr)
            : base("Sentence-Distance")
        {
            var distance = instance.Anaphora.Begin.Line - instance.Antecedent.Begin.Line;
            SetContinuousValue(distance);
        }
    }
}
