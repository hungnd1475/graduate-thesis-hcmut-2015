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
            WikiDataDictionary wikiData, UmlsDataDictionary umlsData)
            :base(size:20, classValue: classValue)
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
            this[8] = new StringMatchFeature(instance);

            this[9] = new PositionFeature(instance, emr);
            this[10] = new IndicatorFeature(instance);
            this[11] = new TemporalFeature(instance, emr);
            this[12] = new SectionFeature(instance, emr);
            this[13] = new ModifierFeature(instance, emr);

            this[14] = new WikiMatchFeature(anaWiki, anteWiki);
            this[15] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[16] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);

            this[17] = new AnatomyFeature(instance, umlsData);
            this[18] = new EquipmentFeature(instance, umlsData);
            this[19] = new ProcedureMatch(instance);
        }
    }
}
