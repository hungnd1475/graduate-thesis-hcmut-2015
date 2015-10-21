using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Service;
    class WikiMatchFeature : Feature
    {
        public WikiMatchFeature(WikiData anaData, WikiData anteData)
            : base("Wiki-Match", 2, 0)
        {
            if (anaData == null || anteData == null)
            {
                return;
            }

            if(anaData.title.Equals(anteData.title, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
