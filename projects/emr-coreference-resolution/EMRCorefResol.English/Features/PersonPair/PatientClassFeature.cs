using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PatientClassFeature : Feature
    {
        public PatientClassFeature(PersonPair instance, IPatientDeterminer patientDeterminer)
            : base("Patient-Class", 2)
        {
            var anaIsPatient = patientDeterminer.IsPatient(instance.Anaphora);
            var anteIsPatient = patientDeterminer.IsPatient(instance.Antecedent);

            if (anaIsPatient == null || anteIsPatient == null)
            {
                // unknown
                SetCategoricalValue(0);
            }
            else if (anaIsPatient.Value && anteIsPatient.Value)
            {
                // true
                SetCategoricalValue(1);
            }
            else
            {
                // false
                SetCategoricalValue(0);
            }
        }
    }
}
