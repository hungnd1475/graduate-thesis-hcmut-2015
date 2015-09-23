using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emr_corefsol_service.Response
{
    public class CustomResponse
    {
        /// <summary>
        /// Success or Fail
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// Data return from action
        /// </summary>
        public Object data { get; set; }

        /// <summary>
        /// Error message if fail
        /// </summary>
        public string message { get; set; }

        public CustomResponse(bool s, Object d, string m)
        {
            success = s;
            data = d;
            message = m;
        }
    }
}