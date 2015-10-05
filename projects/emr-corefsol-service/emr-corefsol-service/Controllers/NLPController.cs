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
        private static readonly INLPHelper NLP_HELPER;

        static NLPController()
        {
            NLP_HELPER = new OpenNLPHelper(HostingEnvironment.MapPath(@"~\app_data\models\OpenNLP\"));
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
            var pos = NLP_HELPER.POSTag(term);

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
            var tokens = NLP_HELPER.Tokenize(term);

            if(tokens == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, tokens, null);
        }

        /// <summary>
        /// Get Chunking from sentence
        /// </summary>
        /// <param name="term">Sentence need to be chunked</param>
        /// <returns></returns>
        [ActionName("Chunk")]
        public CustomResponse GetChunks(string term)
        {
            var chunks = NLP_HELPER.Chunk(term);

            if (chunks == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, chunks, null);
        }
    }
}
