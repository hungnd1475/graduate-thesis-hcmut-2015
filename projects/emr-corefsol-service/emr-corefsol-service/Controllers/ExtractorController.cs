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
            TEMPORAL_HELPER = new TemporalHelper();
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
            //var date = TEMPORAL_HELPER.GetTemporalValue("E:\\graduate-thesis-hcmut-2015\\dataset\\i2b2_Beth_Train_Release.tar\\i2b2_Beth_Train\\Beth_Train\\docs\\clinical-103.txt", "His central line and chest tubes were removed on post-op day #2 and was transferred to the floor in stable condition .\r");

            if (date == null)
            {
                return new CustomResponse(false, null, "Cannot get Temporal value");
            }

            return new CustomResponse(true, date, null);
        }
    }
}
