using HCMUT.EMRCorefResol.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    public class EnglishClasFeatureExtractor : IFeatureExtractor
    {
        public EMR EMR { get; set; }

        public CorefChainCollection GroundTruth { get; set; }

        private readonly IPatientDeterminer _patientDeterminer;

        private readonly Dictionary<Concept, IFeatureVector> _personCache
            = new Dictionary<Concept, IFeatureVector>();

        private readonly object _syncRoot = new object();

        public EnglishClasFeatureExtractor(IClassifier classifier)
        {
            _patientDeterminer = new ClasPatientDeterminer(classifier, this);
            KeywordService.LoadKeywords();
        }

        public IFeatureVector Extract(TestPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new TestPairFeatures(instance, EMR, classValue);
            //return null;
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            lock (_syncRoot)
            {
                if (!_personCache.ContainsKey(instance.Concept))
                {
                    var classValue = -1d;
                    if (GroundTruth != null)
                    {
                        var patientChain = GroundTruth.GetPatientChain();
                        classValue = patientChain != null ? (patientChain.Contains(instance.Concept) ? 1.0 : 0.0) : 2.0;
                    }
                    _personCache.Add(instance.Concept, new PersonInstanceFeatures(instance, EMR, classValue));
                }
            }
            return _personCache[instance.Concept];
        }

        public IFeatureVector Extract(PronounInstance instance)
        {
            var classValue = -1d;
            if (GroundTruth != null)
            {
                var corefChain = GroundTruth.FindChainContains(instance.Concept);
                classValue = corefChain != null ? (double)corefChain.Type : 0;
            }
            return new PronounInstanceFeatures(instance, EMR, classValue);
        }

        public IFeatureVector Extract(TreatmentPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new TreatmentPairFeatures(instance, EMR, classValue);
            //return null;
        }

        public IFeatureVector Extract(ProblemPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new ProblemPairFeatures(instance, EMR, classValue);
            //return null;
        }

        public IFeatureVector Extract(PersonPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new PersonPairFeatures(instance, EMR, _patientDeterminer, classValue);
        }

        public void ClearCache()
        {
            _personCache.Clear();
        }
    }
}
