using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            DateTime anaDate;
            DateTime anteDate;

            try
            {
                anaDate = DateTime.Parse(anaTemporal);
                anteDate = DateTime.Parse(anteTemporal);
            } catch(Exception e)
            {
                return;
            }

            if (anaDate.Equals(anteDate))
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
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
