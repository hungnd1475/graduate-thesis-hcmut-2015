using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PreceededNonPatientFeature : Feature
    {
        public PreceededNonPatientFeature(PersonInstance instance, EMR emr, IPatientDeterminer patientDeterminer)
            : base("Preceeded-NonPatient", 2, 0)
        {
            var index = emr.Concepts.IndexOf(instance.Concept);

            if (index > 0)
            {
                var preceeded = emr.Concepts[index - 1];
                var isPatient = patientDeterminer.IsPatient(preceeded);

                if (isPatient.HasValue && isPatient.Value)
                {
                    SetCategoricalValue(1);
                }
            }
        }
    }
}
