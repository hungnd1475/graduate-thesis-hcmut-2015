using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class TemporalFeature : Feature
    {
        private const int MAX_PREVIOUS_LINE_COUNT = 5;

        public TemporalFeature(IConceptPair instance, EMR emr, TemporalDataDictionary temporalData)
            : base("Temporal-Feature", 3, 2)
        {
            var line = emr.GetLine(instance.Anaphora).Replace("\n", "").Replace("\r", "");

            var anaTemporal = temporalData.Get(line);
            if(string.IsNullOrEmpty(anaTemporal))
            {
                anaTemporal = CheckPreviousLine(instance.Anaphora, emr, temporalData);
                if (string.IsNullOrEmpty(anaTemporal))
                {
                    return;
                }
            }

            line = emr.GetLine(instance.Antecedent).Replace("\n", "").Replace("\r", "");
            var anteTemporal = temporalData.Get(line);
            if(string.IsNullOrEmpty(anteTemporal))
            {
                anteTemporal = CheckPreviousLine(instance.Antecedent, emr, temporalData);
                if (string.IsNullOrEmpty(anteTemporal))
                {
                    return;
                }
            }

            if (CheckDateTime(anaTemporal, anteTemporal))
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
            }
        }

        private bool CheckDateTime(string date1, string date2)
        {
            DateTime d1, d2;
            var success1 = DateTime.TryParse(date1, out d1);
            var success2 = DateTime.TryParse(date2, out d2);

            if(success1 && success2 && d1.Equals(d2))
            {
                return true;
            } else
            {
                var date1RemovedNumber = Regex.Replace(date1, "[0-9]", "");
                var date1Delimiter = date1RemovedNumber.ToCharArray().Distinct().ToArray();

                var date2RemovedNumber = Regex.Replace(date2, "[0-9]", "");
                var date2Delimiter = date2RemovedNumber.ToCharArray().Distinct().ToArray();

                if (date1.Equals(date2) && date1Delimiter.Length <= 1 && date2Delimiter.Length <= 1)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        private string CheckPreviousLine(Concept c, EMR emr, TemporalDataDictionary temporalDictionary)
        {
            var line = emr.GetLine(c);
            var section = emr.GetSection(c);
            var sectionLines = section.Content.Split('\n');

            for(int i=0; i<sectionLines.Length; i++)
            {
                var curLine = sectionLines[i].Replace("\r", "");
                if (curLine.Equals(line))
                {
                    int num = 1;
                    while((i-num)>=0 && num <= MAX_PREVIOUS_LINE_COUNT)
                    {
                        var value = temporalDictionary.Get(sectionLines[i - num]);
                        if (!string.IsNullOrEmpty(value))
                        {
                            return value;
                        }

                        num++;
                    }

                    break;
                }
            }

            return null;
        }
    }
}
