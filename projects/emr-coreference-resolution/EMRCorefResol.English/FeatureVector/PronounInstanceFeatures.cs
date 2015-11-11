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
        public PronounInstanceFeatures(PronounInstance instance, EMR emr, double classValue)
            : base(size: 14, classValue: classValue)
        {
            this[0] = new FirstPreviousMentionTypeFeature(instance, emr);
            this[1] = new SecondPreviousMentionTypeFeature(instance, emr);
            this[2] = new FirstNextMentionTypeFeature(instance, emr);
            this[3] = new PartOfSpeechFeature(instance);
            this[4] = new SemanticFeature(instance, emr);
            this[5] = new PronounIndexFeature(instance);
            this[6] = new FirstNextMentionDistanceFeature(instance, emr);
            this[7] = new FirstPreviousMentionDistanceFeature(instance, emr);
            this[8] = new SecondPreviousMentionDistanceFeature(instance, emr);
            //this[9] = new FirstChunkBeforeMention(instance, emr);
            //this[10] = new FirstChunkAfterMention(instance, emr);
            this[9] = new FirstChunkBeforeIsPreposition(/*(FirstChunkBeforeMention)this[9]*/new FirstChunkBeforeMention(instance, emr));
            this[10] = new FirstNextVerb(instance, emr, KeywordService.Instance.VERBS_AFTER_WORDS);

            this[11] = new First123WordsBoW(instance, emr, true, KeywordService.Instance.PRONOUN_BEFORE_WORDS);
            this[12] = new First123WordsBoW(instance, emr, false, KeywordService.Instance.PRONOUN_AFTER_WORDS);

            this[13] = new AdjacentMentionAfterPronounVP(instance, emr);
        }
    }
}
