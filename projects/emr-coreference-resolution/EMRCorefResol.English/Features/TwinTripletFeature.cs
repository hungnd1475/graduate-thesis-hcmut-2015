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
        public TwinTripletFeature(PersonPair instance)
            :base("Twin-Triplet", 2, 0)
        {
            var searcher = KeywordService.Instance.TWIN_TRIPLET;
            if(searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) &&
                searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }

        public TwinTripletFeature(PersonInstance instance)
            :base("Twin-Triplet", 2, 0)
        {
            var searcher = KeywordService.Instance.TWIN_TRIPLET;
            if (searcher.Match(instance.Concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
