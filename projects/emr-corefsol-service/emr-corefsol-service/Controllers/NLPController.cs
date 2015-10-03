using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;
using System.IO;

using emr_corefsol_service.Response;
using System.Text.RegularExpressions;

namespace emr_corefsol_service.Controllers
{
    using Libs;
    public class NLPController : ApiController
    {
        private readonly INLPHelper _helper = null;

        public NLPController()
        {
            _helper = new OpenNLPHelper(HostingEnvironment.MapPath(@"~\app_data\models\OpenNLP\"));
            //_helper = new SharpNLPHelper(HostingEnvironment.MapPath(@"~\app_data\models\sharpNLP\"));
            //_helper = new StanfordNLPHelper(HostingEnvironment.MapPath(@"~\app_data\models\StanfordNLP\"));
        }


        /// <summary>
        /// GET Gramma Part of Speech from term
        /// </summary>
        /// <param name="term">Term or sentence to tag</param>
        /// <returns></returns>
        /// 
        [ActionName("POS")]
        public CustomResponse GetPOS(string term)
        {
            var pos = _helper.getPOS(term);

            if (pos == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, pos, null);
        }

        /// <summary>
        /// GET Tokens from sentence
        /// </summary>
        /// <param name="term">Sentence need to be tokenized</param>
        /// <returns></returns>
        [ActionName("Token")]
        public CustomResponse GetTokens(string term)
        {
            var tokens = _helper.tokenize(term);

            if(tokens == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, tokens, null);
        }
    }
}
