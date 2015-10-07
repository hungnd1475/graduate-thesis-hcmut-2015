using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LinqToWiki.Generated;
using LinqToWiki.Download;
using System.Text.RegularExpressions;

namespace emr_corefsol_service.Libs
{
    public class WikiHelper
    {
        private readonly Wiki _wiki;

        public WikiHelper()
        {
            _wiki = new Wiki("EMRCorefSol", "en.wikipedia.org", "/w/api.php");
        }

        public Models.WikiData GetWikiInformation(string term)
        {
            try
            {
                var page = (from s in _wiki.Query.search(term)
                            select new { s.title })
                         .ToEnumerable().FirstOrDefault();

                if (page == null)
                {
                    return null;
                }

                var data = _wiki.CreateTitlesSource(page.title)
                    .Select(p => new Models.WikiData
                    (
                        term,
                        p.info.title,
                        p.links().Select(l => l.title).ToList(),
                        p.revisions()
                                .Where(r => r.section == "0")
                                .Select(r => r.value)
                                .ToEnumerable()
                                .FirstOrDefault()
                    ))
                    .ToEnumerable()
                    .FirstOrDefault();

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public string GetPageTitle(string term)
        {
            try
            {
                var page = _wiki.Query.search(term)
                .Select(p => p.title)
                .ToEnumerable()
                .FirstOrDefault();

                return page;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string[] GetBoldName(string term)
        {
            try
            {
                var page = _wiki.Query.search(term)
                .Select(p => p.title)
                .ToEnumerable()
                .FirstOrDefault();

                if (page == null || page.Length <= 0)
                {
                    return null;
                }

                var text = _wiki.CreateTitlesSource(page)
                    .Select(
                        p => p.revisions()
                                .Where(r => r.section == "0")
                                .Select(r => r.value)
                                .ToEnumerable().FirstOrDefault()
                    )
                    .ToEnumerable().FirstOrDefault();

                var pattern = "\'\'\'(.*?)\'\'\'";

                List<string> res = new List<string>();
                foreach(Match m in Regex.Matches(text, pattern))
                {
                    res.Add(m.Groups[1].Value);
                }

                return res.ToArray();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}