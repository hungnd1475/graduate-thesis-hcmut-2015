using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class PersonPairFeatures : FeatureVector
    {
        public PersonPairFeatures(PersonPair instance, EMR emr, IPatientDeterminer patientDeterminer, double classValue)
            : base(size: 20, classValue: classValue)
        {
            this[0] = new PatientClassFeature(instance, patientDeterminer);
            this[1] = new SentenceDistanceFeature(instance, emr);
            this[2] = new MentionDistanceFeature(instance, emr);
            this[3] = new StringMatchFeature(instance);
            this[4] = new LevenshteinDistanceFeature(instance);
            this[5] = new AppositionFeature(instance, emr, this[2].GetContinuousValue());
            this[6] = new NameMatchFeature(instance, emr);
            this[7] = new IInformationFeature(instance);
            this[8] = new YouInformationFeature(instance);
            this[9] = new WeInformationFeature(instance);
            this[10] = new DoctorTitleMatchFeature(instance);
            this[11] = new DoctorGeneralMatch(instance);
            this[12] = new NumberFeature(instance, emr);
            this[13] = new AliasFeature(instance);
            this[14] = new GenderFeature(instance, emr);
            this[15] = new WhoFeatures(instance, emr);
            this[16] = new TwinTripletFeature(instance);
            this[17] = new PronounMatchFeature(instance, emr);
            this[18] = new RelativeKeywordFeature(instance);
            this[19] = new DepartmentKeywordFeature(instance);
        }
    }
}
