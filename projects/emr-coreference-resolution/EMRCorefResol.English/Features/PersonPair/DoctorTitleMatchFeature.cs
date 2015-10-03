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
            var searcher = KeywordService.Instance.DOCTOR_TITLES;
            var kws1 = searcher.SearchKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            if (kws1 == null || kws1.Length < 1)
            {
                SetCategoricalValue(0);
                return;
            }

            var kws2 = searcher.SearchKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            if (kws2 == null || kws2.Length < 1)
            {
                SetCategoricalValue(0);
                return;
            }

            kws1 = kws1.Select(x => x.Replace(".", "")).ToArray();
            kws2 = kws2.Select(x => x.Replace(".", "")).ToArray();

            if (kws1.Intersect(kws2).Count() > 0)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
