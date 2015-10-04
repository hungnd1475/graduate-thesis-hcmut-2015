using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HCMUT.EMRCorefResol.Service
{
    public static class English
    {
        private const string API_URL = "http://localhost:8181/api/";

        public static string[] getPOS(string term)
        {
            HttpUtil http = new HttpUtil();
            var url = API_URL + "nlp/pos?term=" + term;

            CustomResponse res = http.get(url);

            if (!res.success)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static string[] getTokens(string term)
        {
            HttpUtil http = new HttpUtil();
            var url = API_URL + "nlp/token?term=" + term;

            CustomResponse res = http.get(url);

            if (!res.success)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static Definition[] getSyns(string term)
        {
            HttpUtil http = new HttpUtil();
            var url = API_URL + "dictionary/synsets?term=" + term;

            CustomResponse res = http.get(url);

            if (!res.success)
            {
                return null;
            }

            var json = res.data.ToString();
            return JsonConvert.DeserializeObject<Definition[]>(json);
        }

        public static string[] getChunks(string term)
        {
            HttpUtil http = new HttpUtil();
            var url = API_URL + "nlp/chunk?term=" + term;

            CustomResponse res = http.get(url);

            if (!res.success)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }
    }
}
