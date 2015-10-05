using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace emr_corefsol_service.Controllers
{
    using Libs;
    using Response;
    public class WikiController : ApiController
    {
        private static readonly WikiHelper WIKI_HELPER;

        static WikiController()
        {
            WIKI_HELPER = new WikiHelper();
        }

        /// <summary>
        /// Get en.wikipedia page from input term
        /// </summary>
        /// <param name="term">Search string</param>
        /// <returns></returns>
        [ActionName("Page")]
        public CustomResponse GetPage(string term)
        {
            var page = WIKI_HELPER.GetPageTitle(term);

            if(page == null)
            {
                return new CustomResponse(false, null, "Error finding page");
            }

            return new CustomResponse(true, page, null);
        }
    }
}
