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
            :base("Section-Feature", keywords.Count + 1, 0)
        {
            var anaSection = EMRExtensions.GetSection(emr, instance.Anaphora);
            var anteSection = EMRExtensions.GetSection(emr, instance.Antecedent);

            if(anaSection == null || anteSection == null)
            {
                Value[0] = 1d;
                return;
            }

            var anaSectionIndex = keywords.SearchDictionaryIndices(anaSection.Title, KWSearchOptions.WholeWordIgnoreCase)[0];
            var anteSectionIndex = keywords.SearchDictionaryIndices(anteSection.Title, KWSearchOptions.WholeWordIgnoreCase)[0];
            Value[anaSectionIndex + 1] = 1d;
            Value[anteSectionIndex + 1] = 1d;
        }
    }
}
