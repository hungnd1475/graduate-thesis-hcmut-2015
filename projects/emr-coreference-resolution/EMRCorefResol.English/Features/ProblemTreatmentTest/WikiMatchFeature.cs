﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WikiMatchFeature : Feature
    {
        public WikiMatchFeature(IConceptPair instance)
            :base("Wiki-Match", 2, 0)
        {
            string anaPage = Service.English.GetWikiPage(instance.Anaphora.Lexicon);
            string antePage = Service.English.GetWikiPage(instance.Antecedent.Lexicon);

            if(anaPage == null || antePage == null)
            {
                return;
            }

            if(anaPage.Equals(antePage, StringComparison.InvariantCultureIgnoreCase))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
