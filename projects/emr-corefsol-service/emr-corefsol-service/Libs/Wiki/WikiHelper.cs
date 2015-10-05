using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LinqToWiki.Generated;
using LinqToWiki.Download;
namespace emr_corefsol_service.Libs
{
    public class WikiHelper
    {
        private readonly Wiki _wiki;

        public WikiHelper()
        {
            _wiki = new Wiki("EMRCorefSol", "en.wikipedia.org", "/w/api.php");
        }

        public string GetPageTitle(string term)
        {
            try
            {
                var page = _wiki.Query.search(term)
                .Select(p => p.title)
                .ToEnumerable()
                .FirstOrDefault()
                .ToLower();

                return page;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}