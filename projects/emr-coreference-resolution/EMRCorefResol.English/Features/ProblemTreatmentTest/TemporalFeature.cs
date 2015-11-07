using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class TemporalFeature : Feature
    {
        public TemporalFeature(IConceptPair instance, EMR emr, TemporalDataDictionary temporalData)
            : base("Temporal-Feature", 3, 2)
        {
            TemporalData anaTemporal = temporalData.Get(instance.Anaphora, emr);
            if(anaTemporal == null)
            {
                return;
            }

            TemporalData anteTemporal = temporalData.Get(instance.Antecedent, emr);
            if(anteTemporal == null)
            {
                return;
            }

            if (anaTemporal.Value.Equals(anteTemporal.Value))
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
            }
        }
    }
}
