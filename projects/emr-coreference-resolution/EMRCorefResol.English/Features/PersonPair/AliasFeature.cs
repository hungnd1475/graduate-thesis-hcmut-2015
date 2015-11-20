using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Xml;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol.English.Features
{
    class AliasFeature : Feature
    {
        public AliasFeature(IConceptPair instance)
            : base("Alias", 2, 0)
        {
            var stopwords = KeywordService.Instance.STOP_WORDS;
            var anaLex = stopwords.RemoveKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWordIgnoreCase);
            var anteLex = stopwords.RemoveKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWordIgnoreCase);
                  
            var ana = getAbbre(anaLex);
            var ante = getAbbre(anteLex);
            if (anaLex.Equals(ante, StringComparison.InvariantCultureIgnoreCase) ||
                anteLex.Equals(ana, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }

        private string getAbbre(string raw)
        {
            if (!string.IsNullOrEmpty(raw))
            {
                var arr = raw.ToLower().Split(' ');
                arr = arr.Select(i => i[0].ToString()).ToArray();

                return string.Join("", arr);
            }
            return string.Empty;
        }
    }
}
