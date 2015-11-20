using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class GeneralDoctorKeywordFeature : Feature
    {
        public GeneralDoctorKeywordFeature(PersonInstance instance)
            : base("GeneralDoctor-Keyword", 2, 0)
        {
            var seacher = KeywordService.Instance.GENERAL_DOCTOR;

            if (seacher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
