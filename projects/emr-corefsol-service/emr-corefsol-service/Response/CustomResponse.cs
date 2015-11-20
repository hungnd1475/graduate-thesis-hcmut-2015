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
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Data return from action
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Error message if fail
        /// </summary>
        public string Message { get; set; }

        public CustomResponse(bool s, object d, string m)
        {
            IsSuccess = s;
            Data = d;
            Message = m;
        }
    }
}