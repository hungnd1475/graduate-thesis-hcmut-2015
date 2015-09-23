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
            var tagged = Service.English.getPOS(instance.Concept.Lexicon);

            switch (tagged[0].Split('/')[1])
            {
                case "DT":
                    Value = 1.0;
                    break;
                case "WDT":
                    Value = 2.0;
                    break;
                case "PRP":
                    Value = 3.0;
                    break;
                default:
                    Value = 0.0;
                    break;
            }
        }
    }
}
