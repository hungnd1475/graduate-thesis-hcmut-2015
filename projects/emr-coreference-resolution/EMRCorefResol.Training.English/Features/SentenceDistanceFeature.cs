using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Training.English.Features
{
    class SentenceDistanceFeature : Feature
    {
        public SentenceDistanceFeature(IConceptPair instance, EMR emr)
            : base("Sentence-Distance")
        {
            var begin = emr.EndIndexOf(instance.Antecedent);
            var end = emr.BeginIndexOf(instance.Anaphora);
            var s = emr.Content.Substring(begin + 1, end - begin - 1);

            Value = 0;
            int index = 0;

            while (true)
            {
                index = s.IndexOf(" .", index);
                if (index >= 0)
                {
                    index += 2;
                }
                else
                {
                    break;
                }
                Value += 1;
            }
        }
    }
}
