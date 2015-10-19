using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class TwinTripletFeature : Feature
    {
        public TwinTripletFeature(IConceptPair instance)
            : base("Twin-Triplet", 2, 0)
        {
            var searcher = KeywordService.Instance.TWIN_TRIPLET;
            string anteLex = instance.Antecedent.Lexicon, anaLex = instance.Anaphora.Lexicon;

            if (searcher.Match(anteLex, KWSearchOptions.WholeWordIngoreCase) &&
                searcher.Match(anaLex, KWSearchOptions.WholeWordIngoreCase))
            {
                if (string.Equals(anteLex, anaLex))
                    SetCategoricalValue(1);
            }
        }

        public TwinTripletFeature(ISingleConcept instance)
            : base("Twin-Triplet", 2, 0)
        {
            var searcher = KeywordService.Instance.TWIN_TRIPLET;
            if (searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWordIngoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
