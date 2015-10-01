using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.English.Properties;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class DoctorTitleMatchFeature : Feature
    {
        public DoctorTitleMatchFeature(PersonPair instance)
            : base("DoctorTitle-Match", 2)
        {
            /*string kw = null;
            var drTitles = Settings.Default.DoctorTitles;

            foreach (var tt in drTitles)
            {
                if (instance.Antecedent.Lexicon.ToLower().Contains(tt))
                {
                    kw = tt;
                    break;
                }
            }

            SetCategoricalValue((kw != null && instance.Anaphora.Lexicon.ToLower().Contains(kw)) ? 1 : 0);*/

            var searcher = KeywordService.Instance.DOCTOR_TITLES;
            var kws = searcher.SearchKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            if (kws == null || kws.Length < 1)
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
