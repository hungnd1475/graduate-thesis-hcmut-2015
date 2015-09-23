using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    public class EnglishTrainingFeatureExtractor : ITrainingFeatureExtractor
    {
        public EMR EMR { get; set; }
        public CorefChainCollection GroundTruth { get; set; }

        public IFeatureVector Extract(PronounInstance instance)
        {
            var corefChain = GroundTruth.FindChainContains(instance.Concept);
            var classValue = corefChain != null ? (double)corefChain.Type : 0;
            return new PronounInstanceFeatures(instance, EMR, GroundTruth, classValue);
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            var patientChain = GroundTruth.GetPatientChain();
            var classValue = patientChain != null ? (patientChain.Contains(instance.Concept) ? 1.0 : 0.0) : 2.0;
            return new PersonInstanceFeatures(instance, EMR, GroundTruth, classValue);
        }

        public IFeatureVector Extract(TreatmentPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(TestPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(ProblemPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(PersonPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new PersonPairFeatures(instance, EMR, GroundTruth, classValue);
        }
    }
}
