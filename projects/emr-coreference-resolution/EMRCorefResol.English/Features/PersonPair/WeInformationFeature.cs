using HCMUT.EMRCorefResol.Keywords;
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
            new RegexKeywordDictionary("we", "us", "our", "ours", "ourselves");

        public WeInformationFeature(PersonPair instance)
            : base("We-Information", 2, 0)
        {
            //if ((string.Equals(instance.Anaphora.Lexicon.ToLower(), "we") &&
            //    string.Equals(instance.Antecedent.Lexicon.ToLower(), "we")))
            //{
            //    SetCategoricalValue(1);
            //}

            if (WE_KWD.Match(instance.Antecedent.Lexicon, true) &&
                WE_KWD.Match(instance.Anaphora.Lexicon, true))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
