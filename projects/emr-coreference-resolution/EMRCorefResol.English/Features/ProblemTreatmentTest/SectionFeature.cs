using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SectionFeature : Feature
    {
        public SectionFeature(IConceptPair instance, EMR emr)
            :base("Section-Feature", 226, 0)
        {
            var anaSection = EMRExtensions.GetSection(emr, instance.Anaphora);
            var anteSection = EMRExtensions.GetSection(emr, instance.Antecedent);

            if(anaSection == null || anteSection == null)
            {
                SetCategoricalValue(0);
                return;
            }

            var sectionPairIndex = GetSectionPairIndex(anaSection.Title, anteSection.Title);
            SetCategoricalValue(sectionPairIndex);
        }

        private int GetSectionPairIndex(string s1, string s2)
        {
            var s1Index = KeywordService.Instance.SECTION_TITLES.SearchDictionaryIndices(s1, Utilities.KWSearchOptions.WholeWordIgnoreCase)[0];
            var s2Index = KeywordService.Instance.SECTION_TITLES.SearchDictionaryIndices(s2, Utilities.KWSearchOptions.WholeWordIgnoreCase)[0];
            var maxSectionNumber = KeywordService.Instance.SECTION_TITLES.Count;
            return (s1Index * maxSectionNumber) + s2Index + 1;
        }
    }
}
