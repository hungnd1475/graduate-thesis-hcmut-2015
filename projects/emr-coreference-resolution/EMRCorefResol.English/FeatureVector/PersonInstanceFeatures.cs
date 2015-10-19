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
        public PersonInstanceFeatures(PersonInstance instance, EMR emr, IPatientDeterminer PatientDeterminer, double classValue)
            : base(size: 21, classValue: classValue)
        {
            var mostGender = Service.English.GetMostGender(emr);

            this[0] = new PronounIFeature(instance);
            this[1] = new PronounYouFeature(instance);
            this[2] = new PronounHeSheFeature(instance, mostGender);
            this[3] = new PronounWeFeature(instance);
            this[4] = new PronounTheyFeature(instance);
            this[5] = new PatientKeywordFeature(instance);
            this[6] = new DoctorKeywordFeature(instance);
            this[7] = new NameFeature(instance, emr);
            this[8] = new WhoFeatures(instance, emr);
            this[9] = new SignedInformationFeature(instance, emr);
            this[10] = new TwinTripletFeature(instance);
            this[11] = new DoctorLastNLineFeature(instance, emr);
            this[12] = new PreceededNonPatientFeature(instance, emr, PatientDeterminer);
            this[13] = new AppositionFeature(instance, emr);
            this[14] = new DoctorTitleKeywordFeature(instance);
            this[15] = new DepartmentKeywordFeature(instance);
            this[16] = new GeneralDepartmentKeywordFeature(instance);
            this[17] = new GeneralDoctorKeywordFeature(instance);
            this[18] = new RelativeKeywordFeature(instance);
            this[19] = new FirstChunkAfterMention(instance, emr);
            this[20] = new FirstChunkBeforeMention(instance, emr);
        }
    }
}
