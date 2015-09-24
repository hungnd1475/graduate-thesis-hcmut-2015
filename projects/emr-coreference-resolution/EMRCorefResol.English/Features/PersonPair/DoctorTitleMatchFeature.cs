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
        public DoctorTitleMatchFeature(PersonPair instance)
            : base("DoctorTitle-Match", 2)
        {
            string kw = null;
            var drTitles = Settings.Default.DoctorTitles;

            foreach (var tt in drTitles)
            {
                if (instance.Antecedent.Lexicon.ToLower().Contains(tt))
                {
                    kw = tt;
                    break;
                }
            }

            SetCategoricalValue((kw != null && instance.Anaphora.Lexicon.ToLower().Contains(kw)) ? 1 : 0);
        }
    }
}
