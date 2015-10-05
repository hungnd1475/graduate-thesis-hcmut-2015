using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    public class EnglishTrainingFeatureExtractor : IFeatureExtractor
    {
        private IPatientDeterminer _patientDeterminer;

        public EMR EMR { get; set; }

        private CorefChainCollection _groundTruth;
        public CorefChainCollection GroundTruth
        {
            get { return _groundTruth; }
            set
            {
                if (_groundTruth != value)
                {
                    _groundTruth = value;
                    _patientDeterminer = new GroundTruthPatientDeterminer(value);
                }
            }
        }

        public bool NeedGroundTruth
        {
            get { return true; }
        }

        public EnglishTrainingFeatureExtractor()
        {
            KeywordService.LoadKeywords();
        }

        public IFeatureVector Extract(PronounInstance instance)
        {
            var corefChain = _groundTruth.FindChainContains(instance.Concept);
            var classValue = corefChain != null ? (double)corefChain.Type : 0;
            return new PronounInstanceFeatures(instance, EMR, classValue);
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            var patientChain = _groundTruth.GetPatientChain();
            var classValue = patientChain != null ? (patientChain.Contains(instance.Concept) ? 1.0 : 0.0) : 2.0;
            return new PersonInstanceFeatures(instance, EMR, classValue);
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
            var classValue = _groundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new PersonPairFeatures(instance, EMR, _patientDeterminer, classValue);
        }

        public void ClearCache()
        { }
    }
}
