using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emr_corefsol_service.Utilities
{
    public class KeywordCombiner
    {
        private List<List<string>> _keywords;
        public KeywordCombiner(string[] words)
        {
            if(_keywords == null)
            {
                _keywords = new List<List<string>>();
            }
            foreach(string word in words)
            {
                var subList = word.Replace("[", "").Replace("]", "").Split('|').ToList();
                _keywords.Add(subList);
            }
        }

        public string[] Combine()
        {
            //TODO
            List<string> generatedString = new List<string>();

            foreach(string s in _keywords[0])
            {

            }

            return null;
        }
    }
}