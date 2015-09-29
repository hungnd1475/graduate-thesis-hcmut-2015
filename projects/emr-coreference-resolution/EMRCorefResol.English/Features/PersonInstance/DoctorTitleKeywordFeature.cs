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
    class DoctorTitleKeywordFeature : Feature
    {
        public DoctorTitleKeywordFeature(PersonInstance instance)
            : base("DoctorTitle-Keyword", 2, 0)
        {
            var seacher = KeywordService.Instance.DOCTOR_TITLES;

            if (seacher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
