using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

using LAIR.Collections.Generic;
using LAIR.ResourceAPIs.WordNet;
using System.IO;

namespace HCMUT.EMRCorefResol.English.Features
{
    class NumberFeature : Feature
    {
        public NumberFeature(PersonPair instance)
            : base("Number-Information")
        {
            PluralizationService ps = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            //WordNetEngine _wordNetEngine = new WordNetEngine(@"..\..\libs\wordnet\wordnetdb", false);

            string anaSingular = ps.Singularize(instance.Anaphora.Lexicon);
            string anteSingular = ps.Singularize(instance.Antecedent.Lexicon);

            /*Set<SynSet> anaSyns = _wordNetEngine.GetSynSets(anaSingular, WordNetEngine.POS.Noun);
            Set<SynSet> anteSyns = _wordNetEngine.GetSynSets(anteSingular, WordNetEngine.POS.Noun);

            if(anaSyns.Count < 1 || anteSyns.Count < 1)
            {
                Value = 2.0;
                return;
            }*/

            if (!ps.IsSingular(instance.Anaphora.Lexicon) && !ps.IsPlural(instance.Anaphora.Lexicon))
            {
                Value = 2.0;
                return;
            }

            if (!ps.IsSingular(instance.Antecedent.Lexicon) && !ps.IsPlural(instance.Antecedent.Lexicon))
            {
                Value = 2.0;
                return;
            }

            if (ps.IsPlural(instance.Anaphora.Lexicon) && ps.IsPlural(instance.Antecedent.Lexicon))
            {
                Value = 1.0;
                return;
            }

            if (ps.IsSingular(instance.Anaphora.Lexicon) && ps.IsSingular(instance.Antecedent.Lexicon))
            {
                Value = 1.0;
                return;
            }

            Value = 0.0;
        }
    }
}
