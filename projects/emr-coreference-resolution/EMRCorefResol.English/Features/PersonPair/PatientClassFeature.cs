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
            : base("Patient-Class", new[] { 1d, 0d, 0d })
        {
            var anaIsPatient = patientDeterminer.IsPatient(instance.Anaphora);
            var anteIsPatient = patientDeterminer.IsPatient(instance.Antecedent);

            if (anaIsPatient == null || anteIsPatient == null)
            {
                Value[0] = 0d;
                Value[1] = 0d;
                Value[2] = 1d;
            }
            else if (anaIsPatient.Value && anteIsPatient.Value)
            {
                Value[0] = 0d;
                Value[1] = 1d;
                Value[2] = 0d;
            }
        }
    }
}
