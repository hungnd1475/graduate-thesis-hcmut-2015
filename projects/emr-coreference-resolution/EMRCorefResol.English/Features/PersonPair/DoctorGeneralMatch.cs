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
