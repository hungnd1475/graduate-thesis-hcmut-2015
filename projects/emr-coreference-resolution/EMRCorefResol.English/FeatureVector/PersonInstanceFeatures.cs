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
        public PersonInstanceFeatures(PersonInstance instance, EMR emr, double classValue)
            : base(size: 14, classValue: classValue)
        {
            this[0] = new PronounIFeature(instance);
            this[1] = new PronounYouFeature(instance);
            this[2] = new PronounHeSheFeature(instance);
            this[3] = new PronounWeFeature(instance);
            this[4] = new PronounTheyFeature(instance);
            this[5] = new PatientKeywordFeature(instance);
            this[6] = new DoctorKeywordFeature(instance);
            this[7] = new NameFeature(instance, emr);
            this[8] = new WhoFeatures(instance, emr);
            this[9] = new SignedInformationFeature(instance, emr);
            this[10] = new TwinTripletFeature(instance);
            this[11] = new DoctorLastNLineFeature(instance, emr);
            this[12] = new PreceededNonPatientFeature(instance, emr);
            this[13] = new AppositionFeature(instance, emr);
        }
    }
}
