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
            :base(size:16, classValue: classValue)
        {
            var anaMedicationInfo = GetMedicationInfo(instance.Anaphora, emr);
            var anteMedicationInfo = GetMedicationInfo(instance.Antecedent, emr);

            this[0] = new WordNetMatchFeature(instance);
            this[1] = new SentenceDistanceFeature(instance);
            this[2] = new ArticleFeature(instance);
            this[3] = new HeadNounFeature(instance);
            this[4] = new ContainFeature(instance);
            this[5] = new CapitolMatchFeature(instance);
            this[6] = new SubstringFeature(instance);
            this[7] = new CosineDistanceFeature(instance);
            this[8] = new StringMatchFeature(instance);

            this[9] = new PositionFeature(instance, emr);
            this[10] = new DrugFeature(anaMedicationInfo, anteMedicationInfo);
            this[11] = new DosageFeature(anaMedicationInfo, anteMedicationInfo);
            this[12] = new FrequencyFeature(anaMedicationInfo, anteMedicationInfo);
            this[13] = new DurationFeature(anteMedicationInfo, anteMedicationInfo);
            this[14] = new TemporalFeature(instance, emr);
            this[15] = new SectionFeature(instance, emr);

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
