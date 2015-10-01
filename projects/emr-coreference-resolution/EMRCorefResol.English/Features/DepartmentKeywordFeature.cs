using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class DepartmentKeywordFeature : Feature
    {
        public DepartmentKeywordFeature(PersonInstance instance)
            : base("Department-Keyword", 2, 0)
        {
            var kw_searcher = KeywordService.Instance.DEPARTMENT_KEYWORDS;
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
        }

        public DepartmentKeywordFeature(PersonPair instance)
            :base("Department-Keyword", 2, 0)
        {
            var searcher = KeywordService.Instance.DEPARTMENT_KEYWORDS;
            var key1 = searcher.Search(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);
            var key2 = searcher.Search(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if (key1.Intersect(key2).Count() > 0)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
