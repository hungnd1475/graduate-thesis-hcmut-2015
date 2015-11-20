using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class WikiData
    {
        public string term;
        public string title;
        public string[] links;
        public string[] bolds;

        public WikiData(string s, string t, string[] l, string[] b)
        {
            term = s;
            title = t;
            links = l;
            bolds = b;
        }

        public override string ToString()
        {
            string l = string.Join("|", links);
            string b = string.Join("|", bolds);
            return $"term=\"{term}\"||title=\"{title}\"||links=[{l}]||bolds=[{b}]";
        }
    }
}
