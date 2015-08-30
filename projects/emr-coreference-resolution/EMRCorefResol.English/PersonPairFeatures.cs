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
        public PersonPairFeatures(PersonPair instance, EMR emr, CorefChainCollection groundTruth, double classValue)
        {
            AddFeature(new PatientClassFeature(instance, groundTruth));
            AddFeature(new SentenceDistanceFeature(instance, emr));
            AddFeature(new MentionDistanceFeature(instance, emr));
            AddFeature(new StringMatchFeature(instance));
            AddFeature(new LevenshteinDistanceFeature(instance));
            ClassValue = classValue;
        }
    }
}
