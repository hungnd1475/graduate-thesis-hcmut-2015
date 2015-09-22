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

        public static int getGender(string name)
        {
            HttpUtil http = new HttpUtil();
            var url = API_URL + "nlp/gender?name=" + name;

            CustomResponse res = http.get(url);

            if (!res.success)
            {
                return 2;
            }

            switch ((string)res.data)
            {
                case "male":
                    return 0;
                case "female":
                    return 1;
                case "unknow":
                    return 2;
                default:
                    return 2;
            }
        }

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
    }
}
