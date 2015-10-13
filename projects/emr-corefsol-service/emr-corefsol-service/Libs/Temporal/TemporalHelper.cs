using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emr_corefsol_service.Libs
{
    using System.Text.RegularExpressions;
    using System.Web.Hosting;
    using Utilities;
    public class TemporalHelper
    {
        private PythonExcuter _excuter = null;
        private List<Regex> _list = null;
        public TemporalHelper(PythonExcuter e)
        {
            _excuter = e;
            var normaliser = HostingEnvironment.MapPath(@"~\app_data\tools\Clinical-norma\normaliser.py");
            _excuter.SetScriptFile(normaliser);

            _list = new List<Regex>();
            _list.Add(new Regex(@"(?:the |her |his |their )?(?:post-|post|day)? ?(?:pod|operative|op|hospital|hsp|day|hd)(?:ly)? ?(?:day |night |afternoon )? ?(?:number|num\.?|#)? ?([0-9][0-9]*)"));
            _list.Add(new Regex(@"(?:the |her |his |their )?([0-9][0-9]*)(?:st|nd|rd|th)? (?:post-|post|day)? ?(?:pod|operative|op|hospital|hsp|day|hd)(?:ly)? (?:day|night|afternoon)?"));
            _list.Add(new Regex(@"(?:the)? ?([0-9][0-9]?)(?:st|nd|rd|th)? ?(-|to|or) ?([0-9][0-9]?)(?:st|nd|rd|th)? (?:post-|post|day)? ?(?:pod|operative|op|hospital|hsp|day|hd)(?:ly)? ?(?:days?|nights?|afternoons?)?"));
            _list.Add(new Regex(@"(?:the |her |his |their )?day (?:of )?(?:the )?(discharge|admission|transfer|evaluation)"));
            _list.Add(new Regex(@"(?:the|her|his|their)? ?day of life ?#? ?([0-9][0-9]*)"));
            _list.Add(new Regex(@"(?:early)? ?(?:post)? ?(?:-)? ?(operative|extubation) ?(?:course)"));
        }

        public string GetTemporalValue(string emrPath, string line)
        {
            try
            {
                //Get explicit date
                _excuter.Excute(new string[] { "normaliser.py", emrPath, line });

                dynamic result = _excuter.GetVariable("res");
                if (result == null)
                {
                    return null;
                }

                //Get inferred date
                if(result[1] != "DATE")
                {
                    return GetInferredDate(emrPath, line);
                }

                return result[2];
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private string GetInferredDate(string emrPath, string line)
        {
            foreach(Regex r in _list)
            {
                var match = r.Match(line);
                if (match.Success)
                {
                    _excuter.Excute(new string[] { "normaliser.py", emrPath, match.Value });
                    dynamic result = _excuter.GetVariable("res");

                    if (result == null)
                    {
                        continue;
                    }

                    if(result[1] == "DATE")
                    {
                        return result[2];
                    }
                }
            }

            return null;
        }
    }
}