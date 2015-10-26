using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;
    using System.IO;

    class TreatmentPairFeatures : FeatureVector
    {
        public TreatmentPairFeatures(TreatmentPair instance, EMR emr, double classValue)
            :base(size:17, classValue: classValue)
        {
            var emrInfo = new FileInfo(emr.Path);
            var datasetRoot = emrInfo.Directory.Parent.FullName;
            var medicationCollections = MedicationInformation.GetMedicationInfo(datasetRoot, emrInfo.Name);

            var anaMedicationInfo = GetMedicationInfo(instance.Anaphora, emr, medicationCollections);
            var anteMedicationInfo = GetMedicationInfo(instance.Antecedent, emr, medicationCollections);

            /*var anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon);
            var anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon);
            var anaWiki = Service.English.GetAllWikiInformation(anaNorm);
            var anteWiki = Service.English.GetAllWikiInformation(anteNorm);*/

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

            /*this[16] = new WikiMatchFeature(anaWiki, anteWiki);
            this[17] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[18] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);*/

            this[16] = new AnatomyFeature(instance);
        }

        private MedicationInfo GetMedicationInfo(Concept c, EMR emr, MedicationInfoCollection meds)
        {
            var line = emr.GetLine(c.Begin.Line);

            foreach(MedicationInfo med in meds)
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
