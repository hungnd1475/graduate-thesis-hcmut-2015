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
        //public List<string> links;
        public List<string> bolds = new List<string>();

        public WikiData(string s, string t, /*List<string> l,*/ string c)
        {
            term = s;
            title = t;
            //links = l;

            var pattern = "\'\'\'(.*?)\'\'\'";
            foreach (Match m in Regex.Matches(c, pattern))
            {
                bolds.Add(m.Groups[1].Value);
            }
        }
    }
}