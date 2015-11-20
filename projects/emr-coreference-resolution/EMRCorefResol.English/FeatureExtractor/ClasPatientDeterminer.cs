using HCMUT.EMRCorefResol.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    class ClasPatientDeterminer : IPatientDeterminer
    {
        private readonly IFeatureExtractor _fExtractor;
        private readonly IClassifier _classifier;

        public ClasPatientDeterminer(IClassifier classifier, IFeatureExtractor fExtractor)
        {
            _classifier = classifier;
            _fExtractor = fExtractor;
        }

        public bool? IsPatient(Concept concept)
        {
            var instance = new PersonInstance(concept);
            var fVector = _fExtractor.Extract(instance);
            var r = _classifier.Classify(instance, fVector).Class;
            return r == 0d ? false : (r == 1d ? true : (bool?)null);
        }
    }
}
