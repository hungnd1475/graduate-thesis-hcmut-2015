using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WikiBoldNameMatchFeature : Feature
    {
        public WikiBoldNameMatchFeature(IConceptPair instance)
            :base("Wiki-BoldName", 2, 0)
        {
            var anaBolds = Service.English.GetWikiBoldName(instance.Anaphora.Lexicon);
            var anteBolds = Service.English.GetWikiBoldName(instance.Antecedent.Lexicon);

            if(anaBolds == null || anteBolds == null)
            {
                return;
            }

            if(anaBolds.Intersect(anteBolds).Count() > 0)
            {
                SetCategoricalValue(1);
            }
        }
    }
}
