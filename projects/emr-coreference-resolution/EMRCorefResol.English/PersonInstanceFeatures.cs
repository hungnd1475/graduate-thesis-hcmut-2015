using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class PersonInstanceFeatures : FeatureVector
    {
        public PersonInstanceFeatures(PersonInstance instance, EMR emr, CorefChainCollection groundTruth, double classValue)
            : base(size: 6, classValue: classValue)
        {
            this[0] = new PronounIFeature(instance);
            this[1] = new PronounYouFeature(instance);
            this[2] = new PronounWeFeature(instance);
            this[3] = new PronounTheyFeature(instance);
            this[4] = new PatientKeywordFeature(instance);
            this[5] = new DoctorKeywordFeature(instance);
        }
    }
}
