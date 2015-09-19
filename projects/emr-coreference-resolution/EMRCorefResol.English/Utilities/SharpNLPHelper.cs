using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;
using System.IO;

namespace HCMUT.EMRCorefResol.English
{
    public sealed class SharpNLPHelper
    {
        private static string modelsURL = @"../../../libs/sharpNLP/Models/";

        private static readonly EnglishMaximumEntropyPosTagger mPostTagger = new EnglishMaximumEntropyPosTagger(modelsURL + "EnglishPOS.nbin", modelsURL + "Parser/tagdict");

        private static readonly EnglishMaximumEntropyTokenizer mTokenizer = new EnglishMaximumEntropyTokenizer(modelsURL + "EnglishTok.nbin");

        private static readonly string[] femNames = null;
        private static readonly string[] malNames = null;

        public static EnglishMaximumEntropyPosTagger TaggerInstance
        {
            get
            {
                return mPostTagger;
            }
        }

        public static EnglishMaximumEntropyTokenizer TokenizerInstance
        {
            get
            {
                return mTokenizer;
            }
        }

        public static string[] MaleNames
        {
            get
            {
                return malNames;
            }
        }

        public static string[] FemalNames
        {
            get
            {
                return femNames;
            }
        }

        static SharpNLPHelper()
        {
            malNames = File.ReadAllLines(modelsURL + "Coref/gen.mal");
            femNames = File.ReadAllLines(modelsURL + "Coref/gen.fem");
        }
    }
}
