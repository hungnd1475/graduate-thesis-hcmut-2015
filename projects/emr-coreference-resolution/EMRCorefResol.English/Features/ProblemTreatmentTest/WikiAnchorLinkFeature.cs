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

            if(anaData.links.Contains(anteData.title) || anteData.links.Contains(anaData.title) ||
                anaData.links.Contains(anteData.term) || anteData.links.Contains(anaData.term))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
