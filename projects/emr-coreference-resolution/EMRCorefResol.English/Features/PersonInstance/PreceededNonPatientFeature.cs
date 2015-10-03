using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PreceededNonPatientFeature : Feature
    {
        public PreceededNonPatientFeature(PersonInstance instance, EMR emr)
            :base("Preceeded-NonPatient", 2, 0)
        {
            var index = emr.Concepts.IndexOf(instance.Concept);

            if(index > 0)
            {
                var preceeded = emr.Concepts[index - 1];
                var isPatient = new PatientKeywordFeature(new PersonInstance(preceeded));

                if(isPatient.GetCategoricalValue() != 1)
                {
                    SetCategoricalValue(1);
                }
            }
        }
    }
}
