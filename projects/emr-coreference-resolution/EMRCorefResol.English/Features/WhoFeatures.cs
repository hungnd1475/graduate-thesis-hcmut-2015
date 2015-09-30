using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WhoFeatures : Feature
    {
        public WhoFeatures(PersonPair instance, EMR emr)
            :base("Who", 2, 0)
        {
            var anaIndex = emr.Concepts.IndexOf(instance.Anaphora);
            var anteIndex = emr.Concepts.IndexOf(instance.Antecedent);

            //Set 2 flag equals false
            var anaFlag = false;
            var anteFlag = false;

            foreach(Concept c in emr.Concepts)
            {
                //No concept before anaphoro and contains anaphora
                if(emr.Concepts.IndexOf(c) > anaIndex && !anaFlag)
                {
                    return;
                }

                //No concept before antecedent and contains antecedent
                if (emr.Concepts.IndexOf(c) > anteIndex && !anteFlag)
                {
                    return;
                }

                //Exist concept before anaphora and contains anaphora
                if (c.Lexicon.Contains(instance.Anaphora.Lexicon) || instance.Anaphora.Lexicon.Contains(c.Lexicon))
                {
                    anaFlag = true;
                }

                //Exist concept before antecedent and contains antecedent
                if (c.Lexicon.Contains(instance.Antecedent.Lexicon) || instance.Antecedent.Lexicon.Contains(c.Lexicon))
                {
                    anteFlag = true;
                }

                //If both exist then setValue and return
                if(anteFlag && anaFlag)
                {
                    SetCategoricalValue(1);
                    return;
                }
            }
        }

        public WhoFeatures(PersonInstance instance, EMR emr)
            :base("Who", 2, 0)
        {
            var index = emr.Concepts.IndexOf(instance.Concept);

            foreach(Concept c in emr.Concepts)
            {
                if(emr.Concepts.IndexOf(c) > index)
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
