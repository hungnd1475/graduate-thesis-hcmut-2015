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
    class DoctorKeywordFeature : Feature
    {
        public DoctorKeywordFeature(PersonInstance instance)
            :base("Doctor-Keyword", 2, 0)
        {
            var seacher = new AhoCorasickKeywordDictionary("doctors.txt");

            if(seacher.Match(instance.Concept.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
