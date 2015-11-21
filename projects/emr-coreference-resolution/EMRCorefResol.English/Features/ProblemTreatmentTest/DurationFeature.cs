using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DurationFeature : Feature
    {
        public DurationFeature(MedData ana, MedData ante)
            :base("Duration-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;

            if(ana.Duration.Length > 0 &&
                ante.Duration.Length > 0 &&
                string.Equals(ana.Duration, ante.Duration, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
