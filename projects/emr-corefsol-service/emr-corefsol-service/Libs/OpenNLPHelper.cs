using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using opennlp.tools.postag;
using opennlp.tools.tokenize;
using System.Web.Hosting;
using java.io;

namespace emr_corefsol_service.Libs
{
    public class OpenNLPHelper : INLPHelper
    {
        private string modelsRoot = null;
        private POSTaggerME _tagger = null;
        private TokenizerME _tokenizer = null;

        public OpenNLPHelper(string rootPath)
        {
            modelsRoot = rootPath;

            var posModel = new POSModel(new File(modelsRoot + "en-pos-maxent.bin"));
            _tagger = new POSTaggerME(posModel);

            var tokModel = new TokenizerModel(new File(modelsRoot + "en-token.bin"));
            _tokenizer = new TokenizerME(tokModel);
        }

        public string[] tokenize(string term)
        {
            return _tokenizer.tokenize(term);
        }

        public string[] getPOS(string term)
        {
            var tokens = _tokenizer.tokenize(term);

            if(tokens == null)
            {
                return null;
            }

            var tags = _tagger.tag(tokens);

            string[] res = new string[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                res[i] = tokens[i] + "/" + tags[i];
            }

            return res;
        }
    }
}