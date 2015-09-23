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
    public class StanfordNLPHelper
    {
        private static readonly string modelsURL = HostingEnvironment.MapPath(@"~\app_data\libs\stanford-corenlp-3.5.2-models");
        private static readonly StanfordCoreNLP pipeline = null;

        static StanfordNLPHelper()
        {
            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, gender");
            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(modelsURL);
            pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);
        }

        public static List<string> getPOS(string term)
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

            return res;
        }

        public static string getGender(string name)
        {
            Annotation document = new Annotation(name);
            pipeline.annotate(document);

            ArrayList tokens = getTokens(document);

            if (tokens == null)
            {
                return null;
            }

            CoreLabel token = (CoreLabel)tokens.get(0);
            var gender = token.get(typeof(MachineReadingAnnotations.GenderAnnotation));

            if(gender == null || gender.ToString().Length < 1)
            {
                return "unknow";
            }

            return gender.ToString().ToLower();
        }

        private static ArrayList getSentences(Annotation document)
        {
            var sentences = document.get(typeof(CoreAnnotations.SentencesAnnotation));
            if(sentences == null)
            {
                return null;
            }

            return sentences as ArrayList;
        }

        private static ArrayList getTokens(Annotation document)
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