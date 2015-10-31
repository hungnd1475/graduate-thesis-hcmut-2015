using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    public class EnglishClasFeatureExtractor : IFeatureExtractor
    {
        private WikiDataDictionary _wikiData;
        private UmlsDataDictionary _umlsData;
        private MedicationInfoCollection _medInfo;

        private readonly IPatientDeterminer _patientDeterminer;

        private readonly ICache<Concept, IFeatureVector> _personCache
            = new UnlimitedCache<Concept, IFeatureVector>();

        private EMR _emr;
        public EMR EMR
        {
            get { return _emr; }
            set
            {
                if (_emr != value)
                {
                    _emr = value;
                    _wikiData = WikiInformation.GetWikiFile(value.Path);
                    _umlsData = UmlsInformation.GetWikiFile(value.Path);
                    _medInfo = MedicationInformation.GetMedicationFile(value.Path);
                }
            }
        }

        public CorefChainCollection GroundTruth { get; set; }

        public bool NeedGroundTruth
        {
            get { return false; }
        }

        public EnglishClasFeatureExtractor(IClassifier classifier)
        {
            _patientDeterminer = new ClasPatientDeterminer(classifier, this);
            KeywordService.LoadKeywords();
        }

        public IFeatureVector Extract(TestPair instance)
        {
            var classValue = GroundTruth != null ? (GroundTruth.IsCoref(instance) ? 1 : 0) : -1;
            return new TestPairFeatures(instance, EMR, classValue, _wikiData, _umlsData);
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            return _personCache.GetValue(instance.Concept, (c) =>
                {
                    var classValue = -1d;
                    if (GroundTruth != null)
                    {
                        var patientChain = GroundTruth.GetPatientChain(KeywordService.Instance.PATIENT_KEYWORDS,
                            KeywordService.Instance.RELATIVES);
                        classValue = patientChain != null ? (patientChain.Contains(c) ? 1 : 0) : 0;
                    }
                    return new PersonInstanceFeatures(instance, EMR, _patientDeterminer, classValue);
                }                
            );
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
            var classValue = GroundTruth != null ? (GroundTruth.IsCoref(instance) ? 1 : 0) : -1;
            return new TreatmentPairFeatures(instance, EMR, classValue, _medInfo, _wikiData, _umlsData);
        }

        public IFeatureVector Extract(ProblemPair instance)
        {
            var classValue = GroundTruth != null ? (GroundTruth.IsCoref(instance) ? 1 : 0) : -1;
            return new ProblemPairFeatures(instance, EMR, classValue, _wikiData, _umlsData);
        }

        public IFeatureVector Extract(PersonPair instance)
        {
            var classValue = GroundTruth != null ? (GroundTruth.IsCoref(instance) ? 1 : 0) : -1;
            return new PersonPairFeatures(instance, EMR, _patientDeterminer, classValue);
        }

        public void ClearCache()
        {
            _personCache.Clear();
            Service.English.ClearCache();
        }
    }
}
