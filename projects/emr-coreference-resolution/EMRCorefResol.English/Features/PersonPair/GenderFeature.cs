using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class GenderFeature : Feature
    {
        public GenderFeature(PersonPair instance, EMR emr)
            : base("Gender-Information", new[] { 1d, 0d, 0d })
        {
            int anaGen = getGender(instance.Anaphora.Lexicon, emr);
            int anteGen = getGender(instance.Antecedent.Lexicon, emr);

            if (anaGen == 2 || anteGen == 2)
            {
                Value[0] = 0d;
                Value[1] = 0d;
                Value[2] = 1d;
                return;
            }
            else if (anaGen == anteGen)
            {
                Value[0] = 0d;
                Value[1] = 1d;
                Value[2] = 0d;
            }
        }

        private int getGender(string name, EMR emr)
        {
            int keyword = containKeyword(name);
            if(keyword != 2)
            {
                return keyword;
            }

            var appeared = appearedBefore(name, emr);
            if(appeared != 2)
            {
                return appeared;
            }

            var fromDB = Service.English.getGender(name);
            if(fromDB != 2)
            {
                return fromDB;
            }

            return 2;
        }

        private int containKeyword(string name)
        {
            string[] male = { "mr", "mr.", "him", "his", "he" };
            string[] female = { "ms", "ms.", "mrs", "mrs.", "her", "she" };
            foreach (string m in male)
            {
                if (checkContain(name, m))
                {
                    return 0;
                }
            }

            foreach (string f in female)
            {
                if (checkContain(name, f))
                {
                    return 1;
                }
            }

            return 2;
        }

        private int appearedBefore(string name, EMR emr)
        {
            foreach(Concept c in emr.Concepts)
            {
                if(c.Type == ConceptType.Person)
                {
                    var nameArr = name.Split(' ');
                    var conceptArr = c.Lexicon.Split(' ');

                    if(conceptArr.Intersect(nameArr).Count() == 0)
                    {
                        continue;
                    }

                    var keyword = containKeyword(c.Lexicon);
                    if (keyword == 2)
                    {
                        continue;
                    }

                    return keyword;
                }
            }
            return 2;
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
