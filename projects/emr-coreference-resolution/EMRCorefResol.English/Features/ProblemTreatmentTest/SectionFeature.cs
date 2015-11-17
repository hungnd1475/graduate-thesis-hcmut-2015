using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class SectionFeature : Feature
    {
        public SectionFeature(IConceptPair instance, EMR emr, IKeywordDictionary keywords)
            :base("Section-Feature", keywords.Count*keywords.Count + 1, 0)
        {
            var anaSection = EMRExtensions.GetSection(emr, instance.Anaphora);
            var anteSection = EMRExtensions.GetSection(emr, instance.Antecedent);

            if(anaSection == null || anteSection == null)
            {
                SetCategoricalValue(0);
                return;
            }

            var sectionPairIndex = GetSectionPairIndex(anaSection.Title, anteSection.Title, keywords);
            SetCategoricalValue(sectionPairIndex);
        }

        private int GetSectionPairIndex(string s1, string s2, IKeywordDictionary keywords)
        {
            var s1Index = keywords.SearchDictionaryIndices(s1, Utilities.KWSearchOptions.WholeWordIgnoreCase)[0];
            var s2Index = keywords.SearchDictionaryIndices(s2, Utilities.KWSearchOptions.WholeWordIgnoreCase)[0];
            var maxSectionNumber = keywords.Count;
            var index = (s1Index * maxSectionNumber) + s2Index + 1;
            return index;
        }
    }
}
