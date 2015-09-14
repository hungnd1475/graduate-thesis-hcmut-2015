using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PartOfSpeechFeature : Feature
    {
        public PartOfSpeechFeature(PronounInstance instance)
            : base("Part-of-Speech")
        {
            var tagger = SharpNLPHelper.TaggerInstance;
            var POS = tagger.Tag(new string[] {instance.Concept.Lexicon});

            Value = -1.0;
            switch (POS[0])
            {
                case "DT":
                    Value = 0.0;
                    break;
                case "WDT":
                    Value = 1.0;
                    break;
                case "PRP":
                    Value = 2.0;
                    break;
            }
        }
    }
}
