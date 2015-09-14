using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FirstPreviousMentionTypeFeature : Feature
    {
        public FirstPreviousMentionTypeFeature(PronounInstance instance, EMR emr)
            : base("FirstPrevious-MentionType")
        {
            int index = emr.Concepts.IndexOf(instance.Concept);

            if(index < 1)
            {
                Value = 4.0;
                return;
            }

            switch(emr.Concepts[index -1].Type)
            {
                case ConceptType.Person:
                    Value = 0.0;
                    break;
                case ConceptType.Problem:
                    Value = 1.0;
                    break;
                case ConceptType.Treatment:
                    Value = 2.0;
                    break;
                case ConceptType.Test:
                    Value = 3.0;
                    break;
                default:
                    Value = 4.0;
                    break;
            }
        }
    }
}
