﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class TestPairFeatures : FeatureVector
    {
        public TestPairFeatures(TestPair instance, EMR emr, double classValue)
            :base(size:14, classValue: classValue)
        {
            this[0] = new WordNetMatchFeature(instance);
            this[1] = new SentenceDistanceFeature(instance);
            this[2] = new ArticleFeature(instance);
            this[3] = new HeadNounFeature(instance);
            this[4] = new ContainFeature(instance);
            this[5] = new CapitolMatchFeature(instance);
            this[6] = new SubstringFeature(instance);
            this[7] = new CosineDistanceFeature(instance);
            this[8] = new StringMatchFeature(instance);

            this[9] = new PositionFeature(instance, emr);
            this[10] = new IndicatorFeature(instance);
            this[11] = new TemporalFeature(instance, emr);
            this[12] = new SectionFeature(instance, emr);
            this[13] = new ModifierFeature(instance, emr);
        }
    }
}
