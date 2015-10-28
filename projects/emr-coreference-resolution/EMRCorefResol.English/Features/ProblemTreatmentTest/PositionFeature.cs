using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PositionFeature : Feature
    {
        public PositionFeature(IConceptPair instance, EMR emr)
            :base("Position-Feature", 3, 2)
        {
            var anaLine = emr.GetLine(instance.Anaphora);
            var anaPos = GetPosition(instance.Anaphora, anaLine);
            if (anaPos == 0) return;

            var anteLine = emr.GetLine(instance.Antecedent);
            var antePos = GetPosition(instance.Antecedent, anteLine);
            if (antePos == 0) return;

            if(anaPos == antePos)
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
            }
        }

        private int GetPosition(Concept c, string line)
        {
            var searcher = KeywordService.Instance.POSITION_KEYWORD;
            var indices = searcher.SearchDictionaryIndices(line, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if(indices.Length == 0)
            {
                return 0;
            }

            switch (indices.Max())
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return indices.Max() + 1;
                case 4:
                case 5:
                    return 5;
                case 6:
                case 7:
                    return 6;
                case 8:
                case 9:
                    return 7;
                case 10:
                case 11:
                    return 8;
                default:
                    return 0;
            }
        }
    }
}
