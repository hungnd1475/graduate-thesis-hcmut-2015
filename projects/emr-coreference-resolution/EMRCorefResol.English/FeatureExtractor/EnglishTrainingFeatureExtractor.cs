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
        private UmlsDataDictionary _umlsData;
        private WikiDataDictionary _wikiData;
        private MedicationInfoCollection _medInfo;
        private TemporalDataDictionary _temporalData;

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
                    _temporalData = TemporalInformation.GetTemporalFile(value.Path);
                    _medInfo = MedicationInformation.GetMedicationFile(value.Path);
                }
            }
        }

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
            var classValue = corefChain != null ? (double)corefChain.Type : 0d;
            return new PronounInstanceFeatures(instance, EMR, classValue);
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            var patientChain = _groundTruth.GetPatientChain(KeywordService.Instance.PATIENT_KEYWORDS,
                KeywordService.Instance.RELATIVES);
            var classValue = patientChain != null ? (patientChain.Contains(instance.Concept) ? 1 : 0) : 0;
            return new PersonInstanceFeatures(instance, EMR, _patientDeterminer, classValue);
        }

        public IFeatureVector Extract(TreatmentPair instance)
        {
            var classValue = _groundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new TreatmentPairFeatures(instance, EMR, classValue, _medInfo, _wikiData, _umlsData, _temporalData);
        }

        public IFeatureVector Extract(TestPair instance)
        {
            var classValue = _groundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new TestPairFeatures(instance, EMR, classValue, _wikiData, _umlsData, _temporalData);
        }

        public IFeatureVector Extract(ProblemPair instance)
        {
            var classValue = _groundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new ProblemPairFeatures(instance, EMR, classValue, _wikiData, _umlsData);
        }

        public IFeatureVector Extract(PersonPair instance)
        {
            var classValue = _groundTruth.IsCoref(instance) ? 1 : 0;
            return new PersonPairFeatures(instance, EMR, _patientDeterminer, classValue);
        }

        public void ClearCache()
        {
            Service.English.ClearCache();
        }
    }
}
