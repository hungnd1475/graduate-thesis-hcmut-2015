using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace emr_corefsol_service.Controllers
{
    using Libs;
    using System.Web.Hosting;
    using Utilities;
    using Response;
    public class ExtractorController : ApiController
    {
        private static readonly TemporalHelper TEMPORAL_HELPER;

        static ExtractorController()
        {
            var pythonLib = HostingEnvironment.MapPath(@"~\Lib");
            var pyEngine = new PythonExcuter(pythonLib);
            TEMPORAL_HELPER = new TemporalHelper(pyEngine);
        }

        /// <summary>
        /// Get Temporal Value from line
        /// </summary>
        /// <param name="path">Path to EMR file</param>
        /// <param name="line">Line to extract data</param>
        /// <returns></returns>
        [ActionName("Temporal")]
        public CustomResponse GetTemporalDate(string path, string line)
        {
            var date = TEMPORAL_HELPER.GetTemporalValue(path, line);

            if (date == null)
            {
                return new CustomResponse(false, null, "Cannot tokenize");
            }

            return new CustomResponse(true, date, null);
        }
    }
}
