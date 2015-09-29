using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class GeneralDepartmentKeywordFeature : Feature
    {
        public GeneralDepartmentKeywordFeature(PersonInstance instance)
            : base("GeneralDepartment-Keyword", 2, 0)
        {
            var searcher = new AhoCorasickKeywordDictionary("general-department.txt");
            
            if(searcher.Match(instance.Concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
