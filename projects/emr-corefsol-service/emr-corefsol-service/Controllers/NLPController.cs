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
        /// GET list of male's names
        /// </summary>
        /// <returns></returns>
        [ActionName("MaleNames")]
        public CustomResponse GetMaleNames()
        {
            return new CustomResponse(true, Libs.SharpNLPHelper.GetMaleNames(), null);
        }

        /// <summary>
        /// GET list of female's names
        /// </summary>
        /// <returns></returns>
        [ActionName("FemaleNames")]
        public CustomResponse GetFemaleNames()
        {
            return new CustomResponse(true, Libs.SharpNLPHelper.GetFemaleNames(), null); ;
        }

        /// <summary>
        /// GET Gramma Part of Speech from term
        /// </summary>
        /// <param name="term">Term or sentence to tag</param>
        /// <returns></returns>
        [ActionName("POS")]
        public CustomResponse GetPOS(string term)
        {
            if(term!=null && term.Length > 0)
            {
                var pos = Libs.SharpNLPHelper.getPOS(term);
                return new CustomResponse(true, pos, null);
            } else
            {
                return new CustomResponse(false, null, "No term submitted");
            }
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

            var gender = Libs.SharpNLPHelper.getGender(name);
            return new CustomResponse(true, gender, null);
        }
    }
}
