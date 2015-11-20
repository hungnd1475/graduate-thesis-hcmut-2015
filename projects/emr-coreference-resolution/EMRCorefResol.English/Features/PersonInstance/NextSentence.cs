using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class NextSentence : Feature
    {
        public NextSentence(PersonInstance instance, EMR emr)
            : base("Next-Sentence", 2, 1)
        {
            var nextSentence = emr.GetLine(instance.Concept.End.Line + 1);
            if (nextSentence != null && KeywordService.Instance.NEXT_SENTENCES.Contains(nextSentence))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
