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
        public PatientKeywordFeature(PersonInstance instance)
            : base("Patient-Keyword", 2, 0)
        {
            var kw_searcher = new AhoCorasickKeywordDictionary("patients.txt");
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
