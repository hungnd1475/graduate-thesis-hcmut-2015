using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class HeadNounFeature : Feature
    {
        public HeadNounFeature(IConceptPair instance)
            :base("HeadNoun", 2, 0)
        {
            var anaHead = Service.English.GetHeadNoun(instance.Anaphora.Lexicon);
            if (anaHead == null) return;
            var anteHead = Service.English.GetHeadNoun(instance.Antecedent.Lexicon);
            if (anteHead == null) return;

            if(anaHead.Equals(anteHead, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
