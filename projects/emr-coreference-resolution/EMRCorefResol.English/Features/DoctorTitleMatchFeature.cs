using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.English.Properties;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DoctorTitleMatchFeature : Feature
    {
        public DoctorTitleMatchFeature(IConceptPair instance)
            : base("DoctorTitle-Match")
        {
            string kw = null;
            var drTitles = Settings.Default.DoctorTitles;
            
            foreach (var k in drTitles)
            {
                if (instance.Antecedent.Lexicon.ToLower().Contains(k))
                {
                    kw = k;
                    break;
                }
            }

            Value = (kw != null && instance.Anaphora.Lexicon.ToLower().Contains(kw)) ? 1.0 : 0.0;
        }
    }
}
