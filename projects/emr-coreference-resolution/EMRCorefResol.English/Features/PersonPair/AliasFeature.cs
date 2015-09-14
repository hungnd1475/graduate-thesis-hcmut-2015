using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Xml;

namespace HCMUT.EMRCorefResol.English.Features
{
    class AliasFeature : Feature
    {
        public AliasFeature(PersonPair instance)
            : base("Alias")
        {
            var ana = getAbbre(instance.Anaphora.Lexicon);
            var ante = getAbbre(instance.Antecedent.Lexicon);
            Value = (instance.Anaphora.Lexicon.Equals(ante) || instance.Antecedent.Lexicon.Equals(ana)) ?
                1.0 : 0.0;
        }

        private string getAbbre(string raw)
        {
            var arr = raw.ToLower().Split(' ');
            arr = arr.Select(i => i[0].ToString()).ToArray();

            return String.Join("", arr);
        }

        private List<string> getDefinitions(string term)
        {
            string url = "http://www.stands4.com/services/v2/abbr.php?uid=4328&tokenid=HAmSoZHuDnOCm0we&term=" + term;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            Stream s = request.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string xml = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode results = doc.DocumentElement.SelectSingleNode("/results");

            List<string> l = new List<string>();
            foreach (XmlNode result in results.ChildNodes)
            {
                l.Add(result.SelectSingleNode("definition").InnerText);
            }
            return l;
        }
    }
}
