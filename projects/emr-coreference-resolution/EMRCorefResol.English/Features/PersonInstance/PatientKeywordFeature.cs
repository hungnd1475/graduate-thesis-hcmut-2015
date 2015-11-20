using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PatientKeywordFeature : Feature
    {
        static readonly IKeywordDictionary YO_KEYWORDS =
            new AhoCorasickKeywordDictionary("yo-", "-yo");

        public PatientKeywordFeature(PersonInstance instance)
            : base("Patient-Keyword", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.PATIENT_KEYWORDS;
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWordIgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
            else if (YO_KEYWORDS.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWordIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
