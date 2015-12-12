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
            WikiDataDictionary wikiData, UMLSDataDictionary umlsData)
            : base(size: 13, classValue: classValue)
        {
            var anaWiki = wikiData.Get(instance.Anaphora.Lexicon);
            var anteWiki = wikiData.Get(instance.Antecedent.Lexicon);
            
            this[0] = new WikiMatchFeature(anaWiki, anteWiki);
            this[1] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);
            this[2] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[3] = new WordNetMatchFeature(instance);           

            //this[4] = new PositionFeature(instance, emr);
            //this[5] = new SectionFeature(instance, emr, KeywordService.Instance.SECTION_TITLES);
            //this[6] = new AnatomyFeature(instance, umlsData);            

            this[4] = new SentenceDistanceFeature(instance);
            this[5] = new ArticleFeature(instance);
            this[6] = new HeadNounFeature(instance);
            this[7] = new ContainFeature(instance);
            this[8] = new CapitolMatchFeature(instance);
            this[9] = new SubstringFeature(instance);
            this[10] = new CosineDistanceFeature(instance);
            this[11] = new StringMatchFeature(instance);
            this[12] = new ProcedureMatch(instance);
        }
    }
}
