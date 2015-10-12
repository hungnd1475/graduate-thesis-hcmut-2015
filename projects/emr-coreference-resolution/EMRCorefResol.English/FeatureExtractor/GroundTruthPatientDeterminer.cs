using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    class GroundTruthPatientDeterminer : IPatientDeterminer
    {
        private readonly CorefChainCollection _groundTruth;

        public GroundTruthPatientDeterminer(CorefChainCollection groundTruth)
        {
            _groundTruth = groundTruth;
        }

        public bool? IsPatient(Concept concept)
        {
            var patientChain = _groundTruth.GetPatientChain(KeywordService.Instance.PATIENT_KEYWORDS,
                KeywordService.Instance.RELATIVES);
            return patientChain?.Contains(concept);
        }
    }
}
