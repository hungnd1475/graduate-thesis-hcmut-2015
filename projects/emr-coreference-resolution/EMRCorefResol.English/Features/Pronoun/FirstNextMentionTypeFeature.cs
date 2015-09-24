using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FirstNextMentionTypeFeature : Feature
    {
        public FirstNextMentionTypeFeature(PronounInstance instance, EMR emr)
            : base("FirstNext-MentionType", 5, 4)
        {
            int index = emr.Concepts.IndexOf(instance.Concept);

            if (index == emr.Concepts.Count - 1)
            {
                SetCategoricalValue(4);
                return;
            }

            switch (emr.Concepts[index + 1].Type)
            {
                case ConceptType.Person:
                    SetCategoricalValue(0);
                    break;
                case ConceptType.Problem:
                    SetCategoricalValue(1);
                    break;
                case ConceptType.Treatment:
                    SetCategoricalValue(2);
                    break;
                case ConceptType.Test:
                    SetCategoricalValue(3);
                    break;
                default:
                    SetCategoricalValue(4);
                    break;
            }
        }
    }
}
