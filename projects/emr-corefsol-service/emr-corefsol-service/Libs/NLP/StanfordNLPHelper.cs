using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using java.util;

using edu.stanford.nlp.pipeline;

using System.Web.Hosting;
using System.IO;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.ie.machinereading.structure;

namespace emr_corefsol_service.Libs
{
    public class StanfordNLPHelper : INLPHelper
    {
        private readonly string modelsURL = null;
        private readonly StanfordCoreNLP pipeline = null;

        public StanfordNLPHelper(string rootPath)
        {
            modelsURL = rootPath;

            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, gender");

            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(modelsURL);
            pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);
        }

        public string[] POSTag(string term)
        {
            Annotation document = new Annotation(term);
            pipeline.annotate(document);

            ArrayList tokens = getTokens(document);

            if (tokens == null)
            {
                return null;
            }

            List<string> res = new List<string>();
            foreach(CoreLabel token in tokens)
            {
                var item = token.value() + "/" + token.get(typeof(CoreAnnotations.PartOfSpeechAnnotation)).ToString();
                res.Add(item);
            }

            return res.ToArray();
        }

        public string getGender(string name)
        {
            Annotation document = new Annotation(name.ToUpper());
            pipeline.annotate(document);

            ArrayList tokens = getTokens(document);

            if (tokens == null)
            {
                return null;
            }

            foreach(CoreLabel token in tokens)
            {
                var gender = token.get(typeof(MachineReadingAnnotations.GenderAnnotation));
                if(gender != null)
                {
                    return gender.ToString().ToLower();
                }
            }

            return "unknow";
        }

        public string[] Tokenize(string term)
        {
            Annotation document = new Annotation(term);
            pipeline.annotate(document);
            ArrayList tokens = getTokens(document);

            if(tokens == null)
            {
                return null;
            }

            List<string> res = new List<string>();
            foreach(CoreLabel token in tokens)
            {
                res.Add(token.value());
            }

            return res.ToArray();
        }

        public string[] Chunk(string term)
        {
            throw new NotImplementedException();
        }

        public string HeadNoun(string term)
        {
            throw new NotImplementedException();
        }

        private ArrayList getSentences(Annotation document)
        {
            var sentences = document.get(typeof(CoreAnnotations.SentencesAnnotation));
            if(sentences == null)
            {
                return null;
            }

            return sentences as ArrayList;
        }

        private ArrayList getTokens(Annotation document)
        {
            var tokens = document.get(typeof(CoreAnnotations.TokensAnnotation));
            if(tokens == null)
            {
                return null;
            }

            return tokens as ArrayList;
        }
    }
}