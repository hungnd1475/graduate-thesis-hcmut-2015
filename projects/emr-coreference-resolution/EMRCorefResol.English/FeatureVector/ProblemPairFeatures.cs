using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using Features;
    using Service;
    class ProblemPairFeatures : FeatureVector
    {
        public ProblemPairFeatures(ProblemPair instance, EMR emr, double classValue,
            WikiDataDictionary wikiData, UmlsDataDictionary umlsData)
            : base(size: 14, classValue: classValue)
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
            this[10] = new SectionFeature(instance, emr);

            this[10] = new WikiMatchFeature(anaWiki, anteWiki);
            this[11] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[12] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);

            this[13] = new AnatomyFeature(instance, umlsData);
        }
    }
}
