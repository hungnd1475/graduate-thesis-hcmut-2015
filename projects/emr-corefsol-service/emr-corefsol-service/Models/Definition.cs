using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LAIR.ResourceAPIs.WordNet;
namespace emr_corefsol_service.Models
{
    public class Definition
    {
        public WordType POS;
        public string Gloss;
        public string[] Words;

        public Definition(WordType p, string g, string[] w)
        {
            POS = p;
            Gloss = g;
            Words = w;
        }

        public Definition(SynSet s)
        {
            switch (s.POS)
            {
                case WordNetEngine.POS.None:
                    POS = Models.WordType.None;
                    break;
                case WordNetEngine.POS.Noun:
                    POS = Models.WordType.Noun;
                    break;
                case WordNetEngine.POS.Verb:
                    POS = Models.WordType.Verb;
                    break;
                case WordNetEngine.POS.Adjective:
                    POS = Models.WordType.Adjective;
                    break;
                case WordNetEngine.POS.Adverb:
                    POS = Models.WordType.Adverb;
                    break;
                default:
                    POS = Models.WordType.None;
                    break;
            }

            Gloss = s.Gloss;
            Words = s.Words.ToArray();
        }
        public override string ToString()
        {
            return "(" + POS + "): " + Gloss;
        }
    }
}