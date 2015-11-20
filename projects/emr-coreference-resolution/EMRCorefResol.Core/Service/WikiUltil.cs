using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    using System.Web;
    using System.Xml;
    using Utilities;
    class WikiUltil
    {
        private const string WIKI_URL = "http://localhost:8080/wikipedia-miner/services/";

        private readonly HttpUtil _http;

        public WikiUltil()
        {
            _http = new HttpUtil();
        }

        public string QueryTitle(string term)
        {
            var url = WIKI_URL + $"search?query=" + HttpUtility.UrlEncode(term);
            var res = _http.RequestRaw(url);

            if (res == null) return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(res);

            var senses = xmlDoc.GetElementsByTagName("Sense");
            if(senses.Count == 0)
            {
                return null;
            }

            string bestResult = "";
            double highestProbability = 0.0;
            foreach(XmlNode sense in senses)
            {
                var id = sense.Attributes["id"].Value;
                var priorProbability = double.Parse(sense.Attributes["priorProbability"].Value, System.Globalization.CultureInfo.InvariantCulture);

                if(priorProbability > highestProbability)
                {
                    highestProbability = priorProbability;
                    bestResult = id;
                }

            }

            return bestResult;
        }

        public WikiData QueryPageInfo(string term)
        {
            var pageId = QueryTitle(term);
            if (string.IsNullOrEmpty(pageId))
            {
                return null;
            }

            var url = WIKI_URL + "exploreArticle?outLinks=true&labels=true&id=" + HttpUtility.UrlEncode(pageId);
            var res = _http.RequestRaw(url);

            if (res == null || res.Length <= 0) return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(res);

            var response = xmlDoc.GetElementsByTagName("Response");
            if (response[0].InnerXml == null || response[0].InnerXml.Length <= 0)
            {
                return null;
            }

            var pageTitle = response[0].Attributes["title"].Value.ToLower();

            var label = new List<string>();
            var labels = xmlDoc.GetElementsByTagName("Label");
            foreach (XmlNode node in labels)
            {
                label.Add(node.InnerText.ToLower());
            }

            var outLink = new List<string>();
            var links = xmlDoc.GetElementsByTagName("OutLink");
            foreach (XmlNode node in links)
            {
                outLink.Add(node.Attributes["title"].Value.ToLower());
            }

            return new WikiData(term, pageTitle, outLink.ToArray(), label.ToArray());
        }


        public WikiData GetPageInfoByTitle(string title)
        {
            var url = WIKI_URL + "exploreArticle?outLinks=true&labels=true&title=" + HttpUtility.UrlEncode(title);
            var res = _http.RequestRaw(url);

            if (res == null || res.Length <=0) return null;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(res);

            var response = xmlDoc.GetElementsByTagName("Response");
            if(response[0].InnerXml == null || response[0].InnerXml.Length <= 0)
            {
                return null;
            }

            var pageTitle = response[0].Attributes["title"].Value.ToLower();

            var label = new List<string>();
            var labels = xmlDoc.GetElementsByTagName("Label");
            foreach(XmlNode node in labels)
            {
                label.Add(node.InnerText.ToLower());
            }

            var outLink = new List<string>();
            var links = xmlDoc.GetElementsByTagName("OutLink");
            foreach(XmlNode node in links)
            {
                outLink.Add(node.Attributes["title"].Value.ToLower());
            }

            return new WikiData(title, pageTitle, outLink.ToArray(), label.ToArray());
        }
    }
}
