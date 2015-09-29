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
            var kw_searcher = new AhoCorasickKeywordDictionary("department.txt");
            var exist = kw_searcher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (exist)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
