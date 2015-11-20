using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class SignedInformationFeature : Feature
    {
        public SignedInformationFeature(PersonInstance instance, EMR emr)
            :base("Sign-Information", 2, 0)
        {
            var line = EMRExtensions.GetLine(emr, instance.Concept);
            var previous = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line -1);

            var searcher = KeywordService.Instance.SIGN_INFORMATION;

            if(searcher.Match(line, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase) ||
                searcher.Match(previous, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
