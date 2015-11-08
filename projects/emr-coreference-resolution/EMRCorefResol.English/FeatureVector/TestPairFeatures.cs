using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;

    class TestPairFeatures : FeatureVector
    {
        public TestPairFeatures(TestPair instance, EMR emr, double classValue,
            WikiDataDictionary wikiData, UmlsDataDictionary umlsData, TemporalDataDictionary temporalData)
            :base(size:19, classValue: classValue)
        {
            //var wikiDictionary = WikiInformation.GetWikiInfo(emr.Path);
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
            //this[8] = new StringMatchFeature(instance);

            this[8] = new PositionFeature(instance, emr);
            this[9] = new IndicatorFeature(instance, umlsData, emr);
            this[10] = new TemporalFeature(instance, emr, temporalData);
            this[11] = new SectionFeature(instance, emr);
            this[12] = new ModifierFeature(instance, emr);

            this[13] = new WikiMatchFeature(anaWiki, anteWiki);
            this[14] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[15] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);

            this[16] = new AnatomyFeature(instance, umlsData);
            this[17] = new EquipmentFeature(instance, umlsData);
            this[18] = new ProcedureMatch(instance);
        }
    }
}
