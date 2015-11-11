using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PreviousSentence : Feature
    {
        public PreviousSentence(PersonInstance instance, EMR emr)
            : base("Prev-Sentence", 2, 0)
        {
            var prevLine = emr.GetLine(instance.Concept.Begin.Line - 1);
            if (prevLine != null && KeywordService.Instance.PREV_SENTENCES.Contains(prevLine))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
