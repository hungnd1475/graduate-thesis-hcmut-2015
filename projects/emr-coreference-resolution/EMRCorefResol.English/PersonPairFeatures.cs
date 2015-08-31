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
            : base(size: 6)
        {
            this[0] = new PatientClassFeature(instance, groundTruth);
            this[1] = new SentenceDistanceFeature(instance, emr);
            this[2] = new MentionDistanceFeature(instance, emr);
            this[3] = new StringMatchFeature(instance);
            this[4] = new LevenshteinDistanceFeature(instance);
            this[5] = new AppositionFeature(instance, emr, this[2].Value);
            ClassValue = classValue;
        }
    }
}
