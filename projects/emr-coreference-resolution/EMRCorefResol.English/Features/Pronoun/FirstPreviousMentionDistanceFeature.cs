using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FirstPreviousMentionDistanceFeature : Feature
    {
        public FirstPreviousMentionDistanceFeature(PronounInstance instance, EMR emr)
            :base("FirstPrevious-Distance", 0.0)
        {
            int index = emr.Concepts.IndexOf(instance.Concept);

            if(index > 0)
            {
                var previous = emr.Concepts[index - 1];
                SetContinuousValue(instance.Concept.Begin.Line - previous.Begin.Line);
            }
        }
    }
}
