using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FirstNextMentionDistanceFeature : Feature
    {
        public FirstNextMentionDistanceFeature(PronounInstance instance, EMR emr)
            :base("FirstNext-Distance", 0.0)
        {
            int index = emr.Concepts.IndexOf(instance.Concept);

            if(index < emr.Concepts.Count - 1)
            {
                var next = emr.Concepts[index + 1];
                SetContinuousValue(next.Begin.Line - instance.Concept.Begin.Line);
            }
        }
    }
}
