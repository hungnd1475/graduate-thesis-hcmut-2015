using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using opennlp.tools.postag;
using opennlp.tools.tokenize;
using opennlp.tools.chunker;
using opennlp.tools.parser;

using System.Web.Hosting;
using java.io;
using opennlp.tools.cmdline.parser;

namespace emr_corefsol_service.Libs
{
    public class OpenNLPHelper : INLPHelper
    {
        private readonly POSModel _POSModel;
        private readonly TokenizerModel _tokenizerModel;
        private readonly ChunkerModel _chunkerModel;
        private readonly ParserModel _parserModel;

        public OpenNLPHelper(string rootPath)
        {
            _POSModel = new POSModel(new File(rootPath + "en-pos-maxent.bin"));
            _tokenizerModel = new TokenizerModel(new File(rootPath + "en-token.bin"));
            _chunkerModel = new ChunkerModel(new File(rootPath + "en-chunker.bin"));
            _parserModel = new ParserModel(new File(rootPath + "en-parser-chunking.bin"));
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

        public string HeadNoun(string term)
        {
            try
            {
                var parser = ParserFactory.create(_parserModel);
                Parse[] topParses = ParserTool.parseLine(term, parser, 1);

                List<Parse> nounPhrase = new List<Parse>();
                if(nounPhrase.Count <= 0)
                {
                    return term;
                }

                foreach (Parse p in topParses)
                {
                    GetNounPhrase(p, ref nounPhrase);
                }

                return nounPhrase[0].getHead().ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void GetNounPhrase(Parse p, ref List<Parse> list)
        {
            if (p.getType().Equals("NP") || p.getType().Equals("."))
            {
                list.Add(p);
            }
            foreach (Parse child in p.getChildren())
            {
                GetNounPhrase(child, ref list);
            }
        }
    }
}