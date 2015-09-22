using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

using emr_corefsol_service.Response;
using emr_corefsol_service.Libs;

namespace emr_corefsol_service.Controllers
{
    public class DictionaryController : ApiController
    {
        /// <summary>
        /// GET WordNet Synset of a word
        /// </summary>
        /// <param name="term">Lookup term</param>
        /// <returns></returns>
        [ActionName("Synsets")]
        public CustomResponse GetSynsets(string term)
        {
            if(term.Length > 0)
            {
                var defs = WordNetHelper.getSynSets(term);
                return new CustomResponse(true, defs, null);
            } else
            {
                return new CustomResponse(false, null, "No term submitted");
            }
        }
    }
}
