using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SemanticFeature : Feature
    {
        const string AWA = "as well as";
        const string AND = "and";
        const string IAT = "in addition to";

        public SemanticFeature(PronounInstance instance, EMR emr)
            : base("Semantic-Feature", 2, 0)
        {
            //var line = emr.GetLine(instance.Concept.Begin.Line);
            //line = line.Split(new string[] { instance.Concept.Lexicon }, StringSplitOptions.None)[0];
            //bool containKeyword = line.Contains("as well as") || line.Contains("and") || line.Contains("in addition to");
            //SetCategoricalValue(containKeyword ? 1 : 0);

            var beginIndex = emr.BeginIndexOf(instance.Concept);

            var s = emr.Content.Substring(beginIndex - AWA.Length - 1, AWA.Length);
            if (string.Equals(s, AWA))
            {
                SetCategoricalValue(1);
                return;
            }

            s = emr.Content.Substring(beginIndex - AND.Length - 1, AND.Length);
            if (string.Equals(s, AND))
            {
                SetCategoricalValue(1);
                return;
            }

            s = emr.Content.Substring(beginIndex - IAT.Length - 1, IAT.Length);
            if (string.Equals(s, IAT))
            {
                SetCategoricalValue(1);
                return;
            }
        }
    }
}
