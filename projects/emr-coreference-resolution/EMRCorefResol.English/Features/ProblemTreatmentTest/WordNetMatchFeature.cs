using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HCMUT.EMRCorefResol.Utilities;
namespace HCMUT.EMRCorefResol.English.Features
{
    class WordNetMatchFeature : Feature
    {
        public WordNetMatchFeature(IConceptPair instance)
            :base("WordNet-Match", 2, 0)
        {
            var anaNorm = EnglishNormalizer.Normalize(instance.Anaphora.Lexicon);
            var anteNorm = EnglishNormalizer.Normalize(instance.Antecedent.Lexicon);

            var anaDefs = Service.English.GetSyncSets(anaNorm);

            if(anaDefs == null)
            {
                return;
            }

            foreach(Service.Definition definition in anaDefs)
            {
                foreach(string word in definition.Words)
                {
                    var comparable = word.Replace('_', ' ');
                    if(anteNorm.Equals(comparable, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }

            var anteDefs = Service.English.GetSyncSets(anteNorm);

            if (anteDefs == null)
            {
                return;
            }

            foreach (Service.Definition definition in anteDefs)
            {
                foreach (string word in definition.Words)
                {
                    var comparable = word.Replace('_', ' ');
                    if (anaNorm.Equals(comparable, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }
        }
    }
}
