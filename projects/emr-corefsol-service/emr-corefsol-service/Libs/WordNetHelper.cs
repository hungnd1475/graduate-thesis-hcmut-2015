using LAIR.ResourceAPIs.WordNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace emr_corefsol_service.Libs
{
    public class WordNetHelper
    {
        private static readonly string dbUrl = HostingEnvironment.MapPath(@"~\app_data\libs\wordnet\wordnetdb");
        private static readonly WordNetEngine wEngine = new WordNetEngine(dbUrl, true);
        private static readonly WordNetSimilarityModel wSimilarity = new WordNetSimilarityModel(wEngine);

        static WordNetHelper()
        {

        }

        public static Models.Definition[] getSynSets(string term)
        {
            var t = wEngine.GetSynSets(term);
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