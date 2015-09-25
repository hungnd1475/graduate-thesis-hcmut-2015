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
    public class NLPController : ApiController
    {
        /// <summary>
        /// GET Gramma Part of Speech from term
        /// </summary>
        /// <param name="term">Term or sentence to tag</param>
        /// <returns></returns>
        [ActionName("POS")]
        public CustomResponse GetPOS(string term)
        {
            var pos = Libs.OpenNLPHelper.getPOS(term);

            if (pos == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, pos, null);
        }

        /// <summary>
        /// GET Gender from name
        /// </summary>
        /// <param name="name">Name of person</param>
        /// <returns>male/female/unknow</returns>
        [ActionName("Gender")]
        public CustomResponse GetGender(string name)
        {
            if(name==null || name.Length <= 0)
            {
                return new CustomResponse(false, null, "No name submitted");
            }

            var gender = Libs.StanfordNLPHelper.getGender(name);

            if (gender == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, gender, null);
        }
    }
}
