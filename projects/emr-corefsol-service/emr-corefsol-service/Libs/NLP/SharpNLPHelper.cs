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
    public class SharpNLPHelper : INLPHelper
    {
        private readonly string modelsURL = null;
        private readonly EnglishMaximumEntropyPosTagger _postTagger = null;
        private readonly EnglishMaximumEntropyTokenizer _tokenizer = null;

        public SharpNLPHelper(string rootPath)
        {
            modelsURL = rootPath;
            _postTagger = new EnglishMaximumEntropyPosTagger(modelsURL + "EnglishPOS.nbin", modelsURL + "Parser/tagdict");
            _tokenizer = new EnglishMaximumEntropyTokenizer(modelsURL + "EnglishTok.nbin");
        }

        public string[] Chunk(string term)
        {
            throw new NotImplementedException();
        }

        public string[] Tokenize(string term)
        {
            if (term==null)
            {
                return null;
            }
            return _tokenizer.Tokenize(term);
        }

        public string[] POSTag(string term)
        {
            var tokenized = _tokenizer.Tokenize(term);

            if(tokenized == null)
            {
                return null;
            }

            var POS = _postTagger.Tag(tokenized);

            string[] res = new string[tokenized.Length];
            for (int i = 0; i < tokenized.Length; i++)
            {
                res[i] = tokenized[i] + "/" + POS[i];
            }

            return res;
        }
    }
}