using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DurationFeature : Feature
    {
        public DurationFeature(MedicationInfo ana, MedicationInfo ante)
            :base("Duration-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;

            if(!string.IsNullOrEmpty(ana.Duration) &&
                !string.IsNullOrEmpty(ante.Duration) &&
                string.Equals(ana.Duration, ante.Duration, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
