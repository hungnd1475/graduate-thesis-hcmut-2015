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
        private readonly POSModel _POSModel;
        private readonly TokenizerModel _tokenizerModel;

        public OpenNLPHelper(string rootPath)
        {
            _POSModel = new POSModel(new File(rootPath + "en-pos-maxent.bin"));
            _tokenizerModel = new TokenizerModel(new File(rootPath + "en-token.bin"));
        }

        public string[] Tokenize(string term)
        {
            if (term == null)
            {
                return null;
            }

            var tokenizer = new TokenizerME(_tokenizerModel);
            return tokenizer.tokenize(term);
        }

        public string[] POSTag(string term)
        {
            if (term == null)
            {
                return null;
            }

            var tokens = Tokenize(term);

            if (tokens == null)
            {
                return null;
            }

            var tagger = new POSTaggerME(_POSModel);
            var tags = tagger.tag(tokens);

            string[] res = new string[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                res[i] = tokens[i] + "/" + tags[i];
            }

            return res;
        }
    }
}