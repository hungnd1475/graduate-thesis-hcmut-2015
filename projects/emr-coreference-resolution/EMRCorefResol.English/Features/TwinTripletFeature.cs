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

            if (searcher.Match(anteLex, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) &&
                searcher.Match(anaLex, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                if (string.Equals(anteLex, anaLex))
                    SetCategoricalValue(1);
            }
        }

        public TwinTripletFeature(ISingleConcept instance)
            : base("Twin-Triplet", 2, 0)
        {
            var searcher = KeywordService.Instance.TWIN_TRIPLET;
            if (searcher.Match(instance.Concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
