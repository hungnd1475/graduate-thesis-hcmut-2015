using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace emr_corefsol_service.Libs
{
    public class SharpNLPHelper
    {
        private static readonly string modelsURL = HostingEnvironment.MapPath(@"~\app_data\libs\sharpNLP\Models\");

        private static readonly EnglishMaximumEntropyPosTagger mPostTagger = new EnglishMaximumEntropyPosTagger(modelsURL + "EnglishPOS.nbin", modelsURL + "Parser/tagdict");
        private static readonly EnglishMaximumEntropyTokenizer mTokenizer = new EnglishMaximumEntropyTokenizer(modelsURL + "EnglishTok.nbin");

        private static readonly string[] femNames = null;
        private static readonly string[] malNames = null;

        static SharpNLPHelper()
        {
            malNames = File.ReadAllLines(modelsURL + "Coref/gen.mal");
            femNames = File.ReadAllLines(modelsURL + "Coref/gen.fem");
        }

        public static string[] GetMaleNames()
        {
            return malNames;
        }

        public static string[] GetFemaleNames()
        {
            return femNames;
        }

        public static string[] getPOS(string term)
        {
            var tokenized = mTokenizer.Tokenize(term);
            var POS = mPostTagger.Tag(tokenized);

            string[] res = new string[tokenized.Length];
            for (int i = 0; i < tokenized.Length; i++)
            {
                res[i] = tokenized[i] + "/" + POS[i];
            }

            return res;
        }

        public static string getGender(string name)
        {
            var normName = name.ToLower();

            foreach (string n in malNames)
            {
                if (Regex.IsMatch(normName, string.Format(@"\b{0}\b", Regex.Escape(n))))
                {
                    return "male";
                }
            }

            foreach (string n in femNames)
            {
                if (Regex.IsMatch(normName, string.Format(@"\b{0}\b", Regex.Escape(n))))
                {
                    return "female";
                }
            }

            return "unknow";
        }
    }
}