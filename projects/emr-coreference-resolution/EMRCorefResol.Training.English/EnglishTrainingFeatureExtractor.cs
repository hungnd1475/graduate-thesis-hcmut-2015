using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Training.English
{
    using Features;

    public class EnglishTrainingFeatureExtractor : ITrainingFeatureExtractor
    {
        public EMR EMR { get; set; }
        public CorefChainCollection GroundTruth { get; set; }

        public EnglishTrainingFeatureExtractor(EMR emr, CorefChainCollection groundTruth)
        {
            EMR = emr;
            GroundTruth = groundTruth;
        }

        public IFeatureVector Extract(PronounInstance instance)
        {
            return null;
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            return null;
        }

        public IFeatureVector Extract(TreatmentPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(SingleInstance instance)
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
            return new PersonPairFeatures(instance, GroundTruth, classValue);
        }
    }
}
