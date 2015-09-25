using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class YouInformationFeature : Feature
    {
        private static IKeywordDictionary YOU_KWD = 
            new AhoCorasickKeywordDictionary("you", "your", "yours", "yourself");

        public YouInformationFeature(PersonPair instance)
            : base("You-Information", 2, 0)
        {
            //if ((string.Equals(instance.Anaphora.Lexicon.ToLower(), "you") &&
            //        string.Equals(instance.Antecedent.Lexicon.ToLower(), "you")))
            //{
            //    SetCategoricalValue(1);
            //}
            
            if (YOU_KWD.Match(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) && 
                YOU_KWD.Match(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
