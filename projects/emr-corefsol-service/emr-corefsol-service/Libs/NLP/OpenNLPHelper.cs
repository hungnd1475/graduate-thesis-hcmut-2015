using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using opennlp.tools.postag;
using opennlp.tools.tokenize;
using opennlp.tools.chunker;

using System.Web.Hosting;
using java.io;

namespace emr_corefsol_service.Libs
{
    public class OpenNLPHelper : INLPHelper
    {
        private readonly POSModel _POSModel;
        private readonly TokenizerModel _tokenizerModel;
        private readonly ChunkerModel _chunkerModel;

        public OpenNLPHelper(string rootPath)
        {
            _POSModel = new POSModel(new File(rootPath + "en-pos-maxent.bin"));
            _tokenizerModel = new TokenizerModel(new File(rootPath + "en-token.bin"));
            _chunkerModel = new ChunkerModel(new File(rootPath + "en-chunker.bin"));
        }

        public string[] Chunk(string term)
        {
            try
            {
                var tokens = Tokenize(term);

                var tagger = new POSTaggerME(_POSModel);
                var pos = tagger.tag(tokens);

                var chunker = new ChunkerME(_chunkerModel);
                var chunks = chunker.chunk(tokens, pos);

                string[] res = new string[tokens.Length];
                for (int i = 0; i < tokens.Length; i++)
                {
                    res[i] = tokens[i] + "|" + chunks[i];
                }

                return res;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public string[] Tokenize(string term)
        {
            try
            {
                var tokenizer = new TokenizerME(_tokenizerModel);
                return tokenizer.tokenize(term);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public string[] POSTag(string term)
        {
            try
            {
                var tokens = Tokenize(term);
                var tagger = new POSTaggerME(_POSModel);
                var tags = tagger.tag(tokens);

                string[] res = new string[tokens.Length];
                for (int i = 0; i < tokens.Length; i++)
                {
                    res[i] = tokens[i] + "|" + tags[i];
                }

                return res;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}