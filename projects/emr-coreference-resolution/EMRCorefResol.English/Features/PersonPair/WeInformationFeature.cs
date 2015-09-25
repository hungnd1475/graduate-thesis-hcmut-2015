using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WeInformationFeature : Feature
    {
        private static readonly IKeywordDictionary WE_KWD =
            new AhoCorasickKeywordDictionary("we", "us", "our", "ours", "ourselves");

        public WeInformationFeature(PersonPair instance)
            : base("We-Information", 2, 0)
        {
            //if ((string.Equals(instance.Anaphora.Lexicon.ToLower(), "we") &&
            //    string.Equals(instance.Antecedent.Lexicon.ToLower(), "we")))
            //{
            //    SetCategoricalValue(1);
            //}

            if (WE_KWD.Match(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) &&
                WE_KWD.Match(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
