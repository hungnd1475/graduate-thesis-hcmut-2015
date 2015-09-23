using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class HttpUtil
    {
        public CustomResponse get(string url)
        {
            WebRequest request = WebRequest.Create(url);
            Stream s = request.GetResponse().GetResponseStream();
            StreamReader r = new StreamReader(s);
            var res = r.ReadToEnd();
            return JsonConvert.DeserializeObject<CustomResponse>(res);
        }
    }
}
