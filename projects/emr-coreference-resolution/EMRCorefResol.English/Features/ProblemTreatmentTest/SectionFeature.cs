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
            :base("Section-Feature")
        {
            var anaSection = EMRExtensions.GetSectionIndex(emr, instance.Anaphora);
            var anteSection = EMRExtensions.GetSectionIndex(emr, instance.Antecedent);

            if(anaSection == 0 || anteSection == 0)
            {
                SetContinuousValue(0);
                return;
            }

            var sectionPairIndex = GetSectionPairIndex(anaSection, anteSection, emr);
            SetContinuousValue(sectionPairIndex);
        }

        private int GetSectionPairIndex(int s1, int s2, EMR emr)
        {
            return (s1 - 1) * emr.Sections.Count + s2;
        }
    }
}
