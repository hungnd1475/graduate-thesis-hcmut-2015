using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WordNetMatchFeature : Feature
    {
        public WordNetMatchFeature(IConceptPair instance)
            :base("WordNet-Match", 2, 0)
        {
            var anaDefs = Service.English.GetSyncSets(instance.Anaphora.Lexicon);

            if(anaDefs == null)
            {
                return;
            }

            foreach(Service.Definition definition in anaDefs)
            {
                foreach(string word in definition.Words)
                {
                    var comparable = word.Replace('_', ' ');
                    if(instance.Antecedent.Lexicon.Equals(comparable, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }

            var anteDefs = Service.English.GetSyncSets(instance.Antecedent.Lexicon);

            if (anteDefs == null)
            {
                return;
            }

            foreach (Service.Definition definition in anteDefs)
            {
                foreach (string word in definition.Words)
                {
                    var comparable = word.Replace('_', ' ');
                    if (instance.Anaphora.Lexicon.Equals(comparable, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }
        }
    }
}
