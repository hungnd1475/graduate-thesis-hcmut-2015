using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PronounIndexFeature : Feature
    {
        public PronounIndexFeature(PronounInstance instance)
            : base("Pronoun-Index", 18, 0)
        {
            var searcher = KeywordService.Instance.PRONOUNS;
            var index = searcher.SearchIndices(instance.Concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord)[0];
            if(index >=0 && index <= 17)
            {
                SetCategoricalValue(index + 1);
            }
        }
    }
}
