using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PatientClassFeature : Feature
    {
        public PatientClassFeature(PersonPair instance, CorefChainCollection groundTruth)
            : base("Patient-Class")
        {
            var patientChain = groundTruth.GetPatientChain();
            Value = (patientChain.Contains(instance.Antecedent) && patientChain.Contains(instance.Anaphora)) ? 1.0 : 0.0;
        }
    }
}
