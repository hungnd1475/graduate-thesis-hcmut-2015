using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class PronounInstanceFeatures : FeatureVector
    {
        public PronounInstanceFeatures(PronounInstance instance, EMR emr, CorefChainCollection groundTruth, double classValue)
            : base(size: 5, classValue: classValue)
        {
            this[0] = new FirstPreviousMentionTypeFeature(instance, emr);
            this[1] = new SecondPreviousMentionTypeFeature(instance, emr);
            this[2] = new FirstNextMentionTypeFeature(instance, emr);
            this[3] = new PartOfSpeechFeature(instance);
            this[4] = new SemanticFeature(instance, emr);
        }
    }
}
