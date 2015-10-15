using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class TreatmentPairFeatures : FeatureVector
    {
        public TreatmentPairFeatures(TreatmentPair instance, EMR emr, double classValue)
            :base(size:2, classValue: classValue)
        {
            var anaMedicationInfo = GetMedicationInfo(instance.Anaphora, emr);
            var anteMedicationInfo = GetMedicationInfo(instance.Antecedent, emr);

            this[0] = new SentenceDistanceFeature(instance);
            this[1] = new TemporalFeature(instance, emr);
        }

        private MedicationInfo GetMedicationInfo(Concept c, EMR emr)
        {
            var line = emr.GetLine(c.Begin.Line);

            foreach(MedicationInfo med in emr.Medications)
            {
                if(string.Equals(line.Replace("\r", ""), med.Line))
                {
                    if(c.Lexicon.ToLower().Contains(med.Drug.ToLower()) ||
                        med.Drug.ToLower().Contains(c.Lexicon.ToLower()))
                    {
                        return med;
                    }
                }
            }

            return null;
        }
    }
}
