using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class HttpUtil
    {
        public CustomResponse Request(string url)
        {
            var request = WebRequest.Create(url);
            var s = request.GetResponse().GetResponseStream();
            using (var r = new StreamReader(s))
            {
                var res = r.ReadToEnd();
                return JsonConvert.DeserializeObject<CustomResponse>(res);
            }
        }

        public string RequestRaw(string url)
        {
            var request = WebRequest.Create(url);
            var s = request.GetResponse().GetResponseStream();
            using (var r = new StreamReader(s))
            {
                var res = r.ReadToEnd();
                return res;
            }
        }

        public CustomResponse PostData(string url, Dictionary<string, string> data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:6740");
                var content = new FormUrlEncodedContent(data);
                var result = client.PostAsync(url, content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<CustomResponse>(resultContent);
            }
        }
    }
}
