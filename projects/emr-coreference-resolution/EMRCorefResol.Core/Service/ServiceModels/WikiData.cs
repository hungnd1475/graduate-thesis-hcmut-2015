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

        public WikiData(string s, string t, string[] l, string[] c)
        {
            term = s;
            title = t;
            links = l;
            bolds = c;
        }
    }
}
