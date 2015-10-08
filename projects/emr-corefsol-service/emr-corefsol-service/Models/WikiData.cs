using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace emr_corefsol_service.Models
{
    public class WikiData
    {
        public string term;
        public string title;
        public List<string> links = new List<string>();
        public List<string> bolds = new List<string>();

        public WikiData(string s, string t, string c)
        {
            term = s;
            title = t;
            if (c.Contains(@"#REDIRECT") || c.Contains(@"#redirect") || c.Contains(@"#Redirect"))
            {
                Match m = Regex.Match(c, @"\[\[(.*?)\]\]");
                title = m.Groups[1].Value;
            }

            var boldPattern = "\'\'\'(.*?)\'\'\'";
            foreach (Match m in Regex.Matches(c, boldPattern))
            {
                var value = m.Groups[1].Value;

                Match m2 = Regex.Match(value, @"\[\[(.*?)\]\]");
                if (m2.Success)
                {
                    bolds.Add(m2.Groups[1].Value);
                } else
                {
                    bolds.Add(value);
                }
            }

            var linkPattnen = @"\[\[(.*?)\]\]";
            foreach (Match m in Regex.Matches(c, linkPattnen))
            {
                var value = m.Groups[1].Value;
                links.Add(value.Split('|')[0]);
            }
        }
    }
}