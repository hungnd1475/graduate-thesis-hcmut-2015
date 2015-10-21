using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Service;
    class WikiBoldNameMatchFeature : Feature
    {
        public WikiBoldNameMatchFeature(WikiData anaData, WikiData anteData)
            :base("Wiki-BoldName", 2, 0)
        {
            if (anaData == null || anteData == null)
            {
                return;
            }

            if (anaData.bolds.Contains(anteData.title) || anteData.bolds.Contains(anaData.title))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
