using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class FrequencyFeature : Feature
    {
        public FrequencyFeature(MedicationInfo ana, MedicationInfo ante)
            :base("Frequency-Feature", 2, 0)
        {
            if (ana == null || ante == null) return;

            if(!string.IsNullOrEmpty(ana.Frequency) &&
                !string.IsNullOrEmpty(ante.Frequency))
            {
                var anaNorm = Normalized(ana.Frequency);
                var anteNorm = Normalized(ante.Frequency);
                if (anaNorm.Equals(anteNorm))
                {
                    SetCategoricalValue(1);
                }
            }
        }

        private string Normalized(string freq)
        {
            var normalized = freq.Replace(".", "").Trim();

            var daily = @"(daily|qod|^a day$|^once daily$|q[ ]{0,}(day|d)|every(.*?)day|q[ ]{0,}24[ ]{0,}(hours|hour|hrs|h)?|(once |1 )?(per|a) day)";
            if (Regex.IsMatch(normalized, daily, RegexOptions.IgnoreCase))
            {
                return "daily";
            }

            var bid = @"(bid|twice daily|q[ ]{0,}12[ ]{0,}(hours|hour|hrs|h)?|(twice|2 time(s)?|2) (a|per) day|times two)";
            if (Regex.IsMatch(normalized, bid, RegexOptions.IgnoreCase))
            {
                return "bid";
            }

            var tid = @"(tid|^3[ ]{0,}x[ ]{0,}(a )?day$|q[ ]{0,}8[ ]{0,}(hours|hour|hrs|h)?|(three|3) time(s)? (a|per) day|every 8 hour(s)?|times three)";
            if (Regex.IsMatch(normalized, tid, RegexOptions.IgnoreCase))
            {
                return "tid";
            }

            var qid = @"(qid|q[ ]{0,}6[ ]{0,}(hours|hour|hrs|h)?|(four|4) time(s)? (a|per) day|times four)";
            if (Regex.IsMatch(normalized, qid, RegexOptions.IgnoreCase))
            {
                return "qid";
            }

            var qam = @"(qam|every morning|in (the )?morning)";
            if (Regex.IsMatch(normalized, qam, RegexOptions.IgnoreCase))
            {
                return "qam";
            }

            var qpm = @"(qpm|every morning|in (the )?morning)";
            if (Regex.IsMatch(normalized, qpm, RegexOptions.IgnoreCase))
            {
                return "qpm";
            }

            var qhs = @"(qhs|at betime)";
            if (Regex.IsMatch(normalized, qhs, RegexOptions.IgnoreCase))
            {
                return "qhs";
            }

            var qtimes = @"q[ ]{0,}(\d+)[ ]{0,}(hours|hour|hrs|h)?|every (\d+) hour(s)?";
            var match = Regex.Match(normalized, qtimes, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var number = match.Groups[1];
                return $"q{number}h";
            }

            //var every = @"every (\d+) (hours|hour|hrs|h)";
            match = Regex.Match(normalized, qtimes, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var number = match.Groups[1];
                return $"q{number}h";
            }

            return normalized.Trim().ToLower();
        }
    }
}
