using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DrugFeature : Feature
    {
        public DrugFeature(MedData ana, MedData ante)
            :base("Drug-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;
            if(ana.Drug.Length > 0 &&
                ante.Drug.Length > 0 && 
                string.Equals(ana.Drug, ante.Drug, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
