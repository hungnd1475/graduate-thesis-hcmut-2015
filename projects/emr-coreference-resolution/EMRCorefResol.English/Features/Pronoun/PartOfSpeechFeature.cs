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
            : base("Part-of-Speech", 4, 3)
        {
            var tagged = Service.English.POSTag(instance.Concept.Lexicon);

            if (tagged != null)
            {
                switch (tagged[0].Split('/')[1])
                {
                    case "DT":
                        SetCategoricalValue(0);
                        break;
                    case "WDT":
                        SetCategoricalValue(1);
                        break;
                    case "PRP":
                        SetCategoricalValue(2);
                        break;
                    default:
                        SetCategoricalValue(3);
                        break;
                }
            }
        }
    }
}
