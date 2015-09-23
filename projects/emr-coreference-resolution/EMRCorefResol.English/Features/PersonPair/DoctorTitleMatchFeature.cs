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
            : base("DoctorTitle-Match", () => calculateValue(instance))
        { }

        private static double[] calculateValue(PersonPair instance)
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

            return (kw != null && instance.Anaphora.Lexicon.ToLower().Contains(kw)) ? new[] { 0d, 1d } : new[] { 1d, 0d };
        }
    }
}
