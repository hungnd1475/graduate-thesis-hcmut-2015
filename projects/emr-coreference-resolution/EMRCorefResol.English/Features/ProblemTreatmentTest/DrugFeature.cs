using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DrugFeature : Feature
    {
        public DrugFeature(MedicationInfo ana, MedicationInfo ante)
            :base("Drug-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;
            if(string.Equals(ana.Drug, ante.Drug, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
