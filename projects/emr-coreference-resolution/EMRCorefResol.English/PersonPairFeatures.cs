using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;
    using SVM;

    class PersonPairFeatures : FeatureVector, ISVMTrainingFeatures
    {
        public PersonPairFeatures(PersonPair instance, EMR emr, CorefChainCollection groundTruth, double classValue)
            : base(size: 14, classValue: classValue)
        {
            this[0] = new PatientClassFeature(instance, groundTruth);
            this[1] = new SentenceDistanceFeature(instance, emr);
            this[2] = new MentionDistanceFeature(instance, emr);
            this[3] = new StringMatchFeature(instance);
            this[4] = new LevenshteinDistanceFeature(instance);
            this[5] = new AppositionFeature(instance, emr, this[2].Value);
            this[6] = new NameMatchFeature(instance);
            this[7] = new IInformationFeature(instance);
            this[8] = new YouInformationFeature(instance);
            this[9] = new WeInformationFeature(instance);
            this[10] = new DoctorTitleMatchFeature(instance);
            this[11] = new DoctorGeneralMatch(instance);
            this[12] = new NumberFeature(instance);
            this[13] = new AliasFeature(instance);
        }

        public void AddTo(SVMProblems problems)
        {
            problems.Add(this);
        }
    }
}
