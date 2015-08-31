using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;

    class SentenceDistanceFeature : Feature
    {
        public SentenceDistanceFeature(IConceptPair instance, EMR emr)
            : base("Sentence-Distance")
        {
            var s = emr.ContentBetween(instance.Antecedent, instance.Anaphora);
            int index = 0;

            while (true)
            {
                index = s.IndexOf(" .", index);
                if (index >= 0) index += 2;
                else break;
                Value += 1;
            }
        }
    }
}
