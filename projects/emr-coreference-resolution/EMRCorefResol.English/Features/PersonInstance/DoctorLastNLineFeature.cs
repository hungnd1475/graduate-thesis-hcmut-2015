using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    /// <summary>
    /// Read EMR up-side-down until keyword found
    /// If concept is Doctor and belong to n last line
    /// </summary>
    class DoctorLastNLineFeature : Feature
    {
        public DoctorLastNLineFeature(PersonInstance instance, EMR emr)
            :base("Doctor-LastNLine", 2, 0)
        {
            var isDoctor = new DoctorKeywordFeature(instance);
            if(isDoctor.GetCategoricalValue() != 1)
            {
                return;
            }


            var sections = emr.Sections;
            var reversed = sections.Reverse().ToArray();
            var searcher = KeywordService.Instance.NLINE_KEYWORD;
            int n = 0;
            for(int i=0; i<reversed.Length; i++)
            {
                if(searcher.Match(reversed[i].Title, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
                {
                    n = reversed[i].Begin;
                    break;
                }
            }

            if(instance.Concept.Begin.Line > n)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
