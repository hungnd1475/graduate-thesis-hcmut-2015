using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SemanticFeature : Feature
    {
        public SemanticFeature(PronounInstance instance, EMR emr)
            : base("Semantic-Feature")
        {
            var line = emr.getLine(instance.Concept.Begin.Line);
            line = line.Split(new string[] { instance.Concept.Lexicon }, StringSplitOptions.None)[0];
            bool containKeyword = line.ToLower().Contains("as well as") || line.ToLower().Contains("and") || line.ToLower().Contains("in addition to");
            Value = containKeyword ? 1.0 : 0.0;
        }
    }
}
