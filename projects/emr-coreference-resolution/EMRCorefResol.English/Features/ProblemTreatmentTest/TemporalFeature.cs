using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class TemporalFeature : Feature
    {
        public TemporalFeature(IConceptPair instance, EMR emr)
            : base("Temporal-Feature", 3, 2)
        {
            var emrPath = new FileInfo(emr.Path).FullName;
            var line = emr.GetLine(instance.Anaphora.Begin.Line);

            var anaTemporal = Service.English.GetTemporalValue(emrPath, line);
            if (anaTemporal == null)
            {
                return;
            }

            line = emr.GetLine(instance.Antecedent.Begin.Line);
            var anteTemporal = Service.English.GetTemporalValue(emrPath, line);
            if (anteTemporal == null)
            {
                return;
            }

            if (anaTemporal.Equals(anteTemporal))
            {
                SetCategoricalValue(1);
            }
            else
            {
                SetCategoricalValue(0);
            }
        }
    }
}
