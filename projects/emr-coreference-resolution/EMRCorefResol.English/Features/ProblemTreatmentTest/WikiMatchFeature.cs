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
        public WikiMatchFeature(IConceptPair instance)
            :base("Wiki-Match", 2, 0)
        {
            string anaPage = English.GetWikiPage(instance.Anaphora.Lexicon);
            string antePage = English.GetWikiPage(instance.Antecedent.Lexicon);

            if(anaPage == null || antePage == null)
            {
                return;
            }

            if(anaPage.Equals(antePage, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }

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
