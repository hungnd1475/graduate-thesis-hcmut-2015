using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    /// <summary>
    /// Concept is a name and appeared before in EMR
    /// </summary>
    class WhoFeatures : Feature
    {
        public WhoFeatures(PersonPair instance, EMR emr)
            :base("Who", 2, 0)
        {
            var whoAna = new WhoFeatures(new PersonInstance(instance.Anaphora), emr);
            var whoAnte = new WhoFeatures(new PersonInstance(instance.Antecedent), emr);

            if(whoAna.Value[1] == 1 && whoAnte.Value[1] == 1)
            {
                SetCategoricalValue(1);
            }
        }

        public WhoFeatures(PersonInstance instance, EMR emr)
            :base("Who", 2, 0)
        {
            var isName = new NameFeature(instance, emr);
            if(isName.Value[0] == 1)
            {
                return;
            }

            var index = emr.Concepts.IndexOf(instance.Concept);

            foreach(Concept c in emr.Concepts)
            {
                if(emr.Concepts.IndexOf(c) >= index)
                {
                    return;
                }

                if (c.Lexicon.Contains(instance.Concept.Lexicon) || instance.Concept.Lexicon.Contains(c.Lexicon))
                {
                    SetCategoricalValue(1);
                    return;
                }
            }
        }
    }
}
