using System;
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
            :base(size:1, classValue: classValue)
        {
            this[0] = new SentenceDistanceFeature(instance);
            //this[1] = new WikiMatchFeature(instance);
        }
    }
}
