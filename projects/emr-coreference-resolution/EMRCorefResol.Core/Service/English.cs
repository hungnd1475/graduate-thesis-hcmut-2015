using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace HCMUT.EMRCorefResol.Service
{
    using Utilities;
    public static class English
    {
        private const string API_URL = "http://localhost:8181/api/";
        private static readonly HttpUtil _http = new HttpUtil();
        private static ICache<string, WikiData> _wikiCache;

        static English()
        {
            _wikiCache = new UnlimitedCache<string, WikiData>();
        }

        public static string[] POSTag(string term)
        {
            var url = API_URL + "nlp/pos?term=" + HttpUtility.UrlEncode(term);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.Data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static string[] Tokenize(string term)
        {
            var url = API_URL + "nlp/token?term=" + HttpUtility.UrlEncode(term);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.Data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static Definition[] GetSyncSets(string term)
        {
            var url = API_URL + "dictionary/synsets?term=" + HttpUtility.UrlEncode(term);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            var json = res.Data.ToString();
            return JsonConvert.DeserializeObject<Definition[]>(json);
        }

        public static string[] GetChunks(string term)
        {
            var url = API_URL + "nlp/chunk?term=" + HttpUtility.UrlEncode(term);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.Data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static string GetWikiPage(string term)
        {
            var url = API_URL + "wiki/page?term=" + HttpUtility.UrlEncode(term);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            return (string)res.Data;
        }

        public static string[] GetWikiBoldName(string page)
        {
            var url = API_URL + "wiki/boldname?page=" + HttpUtility.UrlEncode(page);
            var res = _http.Request(url);

            if (!res.IsSuccess)
            {
                return null;
            }

            return ((System.Collections.IEnumerable)res.Data)
              .Cast<object>()
              .Select(x => x.ToString())
              .ToArray();
        }

        public static WikiData GetAllWikiInformation(string term)
        {
            return _wikiCache.GetValue(term, (string search_term) =>
            {
                var url = API_URL + "wiki/information?term=" + HttpUtility.UrlEncode(search_term);
                var res = _http.Request(url);

                if (!res.IsSuccess)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<WikiData>(res.Data.ToString());
            });
        }
    }
}
