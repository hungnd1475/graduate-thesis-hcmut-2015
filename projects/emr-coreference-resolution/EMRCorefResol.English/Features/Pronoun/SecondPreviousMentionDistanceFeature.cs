using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SecondPreviousMentionDistanceFeature : Feature
    {
        public SecondPreviousMentionDistanceFeature(PronounInstance instance, EMR emr)
            :base("SecondPrevious-Distance", 0.0)
        {
            int index = emr.Concepts.IndexOf(instance.Concept);

            if (index > 1)
            {
                var previous = emr.Concepts[index - 2];
                SetContinuousValue(instance.Concept.Begin.Line - previous.Begin.Line);
            }
        }
    }
}
