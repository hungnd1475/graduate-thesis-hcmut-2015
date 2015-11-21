using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Service;
    class WikiAnchorLinkFeature : Feature
    {
        public WikiAnchorLinkFeature(WikiData anaData, WikiData anteData)
            :base("Wiki-AnchorLink", 2, 0)
        {
            if (anaData == null || anteData == null)
            {
                return;
            }

            if(anaData.Links.Contains(anteData.Title) || anteData.Links.Contains(anaData.Title) ||
                anaData.Links.Contains(anteData.Term) || anteData.Links.Contains(anaData.Term))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
