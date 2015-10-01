using HCMUT.EMRCorefResol.English.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class DoctorGeneralMatch : Feature
    {
        public DoctorGeneralMatch(PersonPair instance)
            : base("Doctor-General-Match", 2, 0)
        {
            /*var drGenerals = Settings.Default.GeneralDoctors;
            var anteLex = instance.Antecedent.Lexicon.ToLower();
            var anaLex = instance.Anaphora.Lexicon.ToLower();
            string kw = null;

            foreach (var dg in drGenerals)
            {
                if (anteLex.Contains(dg))
                {
                    kw = dg;
                    break;
                }
            }

            SetCategoricalValue((kw != null && anaLex.Contains(kw)) ? 1 : 0);*/

            var searcher = KeywordService.Instance.GENERAL_DOCTOR;
            var kws = searcher.SearchKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            if(kws == null || kws.Length < 1)
            {
                SetCategoricalValue(0);
                return;
            }

            searcher = new AhoCorasickKeywordDictionary(kws);
            kws = searcher.SearchKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            if (kws == null || kws.Length < 1)
            {
                SetCategoricalValue(0);
                return;
            }

            SetCategoricalValue(1);
        }
    }
}
