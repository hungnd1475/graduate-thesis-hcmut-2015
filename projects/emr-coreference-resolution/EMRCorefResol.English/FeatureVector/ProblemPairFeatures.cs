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
            : base(size: 3, classValue: classValue)
        {
            //WikiData anaWiki = English.GetAllWikiInformation(instance.Anaphora.Lexicon);
            //WikiData anteWiki = English.GetAllWikiInformation(instance.Antecedent.Lexicon);

            this[0] = new SentenceDistanceFeature(instance);
            this[1] = new PositionFeature(instance, emr);
            this[2] = new SectionFeature(instance, emr);
        }
    }
}
