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
        public TreatmentPairFeatures(TreatmentPair instance, EMR emr, double classValue,
            MedicationInfoCollection medInfo, WikiDataDictionary wikiData, UmlsDataDictionary umlsData)
            :base(size:20, classValue: classValue)
        {
            //var medicationCollections = MedicationInformation.GetMedicationInfo(emr.Path);

            var anaMedicationInfo = GetMedicationInfo(instance.Anaphora, emr, medInfo);
            var anteMedicationInfo = GetMedicationInfo(instance.Antecedent, emr, medInfo);

            //var wikiDictionary = WikiInformation.GetWikiInfo(emr.Path);
            //var anaWiki = wikiDictionary.Get(instance.Anaphora.Lexicon);
            //var anteWiki = wikiDictionary.Get(instance.Antecedent.Lexicon);

            var anaWiki = wikiData.Get(instance.Anaphora.Lexicon);
            var anteWiki = wikiData.Get(instance.Antecedent.Lexicon);

            //var umlsDictionary = UmlsInformation.GetUmlsInfo(emr.Path);

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

            this[16] = new WikiMatchFeature(anaWiki, anteWiki);
            this[17] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[18] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);

            this[19] = new OperationFeature(instance, umlsData);
        }

        private MedicationInfo GetMedicationInfo(Concept c, EMR emr, MedicationInfoCollection meds)
        {
            var line = emr.GetLine(c);

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
