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
            var anaPos = GetPosition(instance.Anaphora.Lexicon);
            if (anaPos == 0)
            {
                return;
            }

            var antePos = GetPosition(instance.Antecedent.Lexicon);
            if (antePos == 0)
            {
                return;
            }

            if(anaPos == antePos)
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
            }
        }

        private int GetPosition(string term)
        {
            var searcher = KeywordService.Instance.POSITION_KEYWORD;
            var indices = searcher.SearchDictionaryIndices(term, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

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
                case 4:
                case 5:
                case 6:
                    return indices.Max() + 1;
                case 7:
                case 8:
                    return 8;
                case 9:
                case 10:
                    return 9;
                case 11:
                case 12:
                    return 10;
                case 13:
                case 14:
                    return 11;
                default:
                    return 0;
            }
        }
    }
}
