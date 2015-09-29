using LAIR.ResourceAPIs.WordNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace emr_corefsol_service.Libs
{
    public class WordNetHelper : IDictionaryHelper
    {
        private readonly string dbUrl = null;
        private readonly WordNetEngine _wEngine = null;
        private readonly WordNetSimilarityModel _wSimilarity = null;

        public WordNetHelper(string rootPath)
        {
            dbUrl = rootPath;
            _wEngine = new WordNetEngine(dbUrl, true);
            _wSimilarity = new WordNetSimilarityModel(_wEngine);
        }

        public Models.Definition[] getSynSets(string term)
        {
            var t = _wEngine.GetSynSets(term);
            Models.Definition[] res = new Models.Definition[t.Count];

            int i = 0;
            foreach (SynSet s in t)
            {
                res[i] = new Models.Definition(s);
                i++;
            }

            return res;
        }
    }
}