using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Training.English.Features
{
    class PersonPairFeatures : FeatureVector
    {
        public PersonPairFeatures(PersonPair instance, EMR emr, CorefChainCollection groundTruth, double classValue)
        {
            AddFeature(new PatientClassFeature(instance, groundTruth));
            AddFeature(new SentenceDistanceFeature(instance, emr));
            ClassValue = classValue;
        }
    }
}
