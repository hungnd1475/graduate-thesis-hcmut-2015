using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;

namespace HCMUT.EMRCorefResol.English
{
    public sealed class SharpNLPHelper
    {
        private static string modelsURL = @"../../../libs/sharpNLP/Models/";

        private static readonly EnglishMaximumEntropyPosTagger mPostTagger = new EnglishMaximumEntropyPosTagger(modelsURL + "EnglishPOS.nbin", modelsURL + "Parser/tagdict");

        private static readonly EnglishMaximumEntropyTokenizer mTokenizer = new EnglishMaximumEntropyTokenizer(modelsURL + "EnglishTok.nbin");

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
    }
}
