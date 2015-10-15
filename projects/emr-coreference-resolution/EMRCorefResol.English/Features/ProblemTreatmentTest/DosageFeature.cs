﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DosageFeature : Feature
    {
        public DosageFeature(MedicationInfo ana, MedicationInfo ante)
            :base("Dosage-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;

            if(string.Equals(ana.DoseAmount, ante.DoseAmount, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(ana.Strength, ante.Strength, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
