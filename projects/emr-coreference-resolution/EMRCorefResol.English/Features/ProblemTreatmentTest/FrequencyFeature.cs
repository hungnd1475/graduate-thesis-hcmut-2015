using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FrequencyFeature : Feature
    {
        public FrequencyFeature(MedicationInfo ana, MedicationInfo ante)
            :base("Frequency-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;

            if(ana.Frequency.Length > 0 &&
                ante.Frequency.Length > 0 && 
                string.Equals(ana.Frequency, ante.Frequency, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
