using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

using emr_corefsol_service.Response;
using emr_corefsol_service.Libs;
using System.Web.Hosting;

namespace emr_corefsol_service.Controllers
{
    public class DictionaryController : ApiController
    {
        private IDictionaryHelper _helper = null;

        public DictionaryController()
        {
            _helper = new WordNetHelper(HostingEnvironment.MapPath(@"~\app_data\models\wordnet"));
        }

        /// <summary>
        /// GET WordNet Synset of a word
        /// </summary>
        /// <param name="term">Lookup term</param>
        /// <returns></returns>
        [ActionName("Synsets")]
        public CustomResponse GetSynsets(string term)
        {
            var defs = _helper.getSynSets(term);

            if(defs == null)
            {
                return new CustomResponse(false, null, "No definition found");
            }

            return new CustomResponse(true, defs, null);
        }
    }
}
