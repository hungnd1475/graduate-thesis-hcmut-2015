using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;
    using SVM;

    class PatientClassFeatures : FeatureVector, ISVMTrainingFeatures
    {
        public PatientClassFeatures(PersonInstance instance, EMR emr, CorefChainCollection groundTruth, double classValue)
            : base(size: 0, classValue: classValue)
        {

        }

        public void AddTo(SVMProblems problems)
        {
            problems.Add(this);
        }
    }
}
