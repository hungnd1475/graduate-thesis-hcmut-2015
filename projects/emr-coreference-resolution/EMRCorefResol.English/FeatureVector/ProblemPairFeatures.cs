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
        public ProblemPairFeatures(ProblemPair instance, EMR emr, double classValue)
            : base(size: 11, classValue: classValue)
        {
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
            this[10] = new SectionFeature(instance, emr);

            /*this[10] = new WikiMatchFeature(anaWiki, anteWiki);
            this[11] = new WikiAnchorLinkFeature(anaWiki, anteWiki);
            this[12] = new WikiBoldNameMatchFeature(anaWiki, anteWiki);*/

            //this[11] = new OperationFeature(instance);
        }
    }
}
