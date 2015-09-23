using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SentenceDistanceFeature : Feature
    {
        public SentenceDistanceFeature(IConceptPair instance, EMR emr)
            : base("Sentence-Distance", new[] { 0d })
        {
            var s = emr.ContentBetween(instance);
            int index = 0;

            while (true)
            {
                index = s.IndexOf(" .", index);
                if (index >= 0) index += 2;
                else break;
                Value[0] += 1;
            }
        }
    }
}
