using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Training.English.Features
{
    class PatientClassFeature : IFeature
    {
        public string Name { get; } = "Patient-class";

        public double Value { get; }

        public PatientClassFeature(PersonPair instance, CorefChainCollection groundTruth)
        {
            var patientChain = groundTruth.GetPatientChain();
            Value = (patientChain.Contains(instance.Antecedent) && patientChain.Contains(instance.Anaphora)) ? 1.0 : 0.0;
        }
    }
}
