using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class WikiData
    {
        public string Term { get; }
        public string Title { get; }
        public string[] Links { get; }
        public string[] Bolds { get; }

        public WikiData(string s, string t, string[] l, string[] b)
        {
            Term = s;
            Title = t;
            Links = l;
            Bolds = b;
        }

        public override string ToString()
        {
            string l = string.Join("|", Links);
            string b = string.Join("|", Bolds);
            return $"term=\"{Term}\"||title=\"{Title}\"||links=[{l}]||bolds=[{b}]";
        }
    }
}
