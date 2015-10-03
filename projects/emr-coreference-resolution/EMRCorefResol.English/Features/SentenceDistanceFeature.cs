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
            : base("Sentence-Distance")
        {
            //var s = emr.ContentBetween(instance);
            //int index = 0;
            //var v = 0d;

            //while (true)
            //{
            //    index = s.IndexOf(" .", index);
            //    if (index >= 0) index += 2;
            //    else break;
            //    v += 1;
            //}

            //SetContinuousValue(v);

            SetContinuousValue(instance.Anaphora.Begin.Line - instance.Antecedent.End.Line);
        }
    }
}
