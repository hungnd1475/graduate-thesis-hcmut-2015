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
            if (anaPos == null)
            {
                return;
            }

            var antePos = GetPosition(instance.Antecedent.Lexicon);
            if (antePos == null)
            {
                return;
            }

            if(anaPos == antePos)
            {
                SetCategoricalValue(1);
            } else
            {
                if(anaPos.Contains(antePos) || antePos.Contains(anaPos))
                {
                    SetCategoricalValue(1);
                } else
                {
                    SetCategoricalValue(0);
                }
            }
        }

        private string GetPosition(string term)
        {
            var searcher = KeywordService.Instance.POSITION_KEYWORD;
            var indices = searcher.SearchDictionaryIndices(term, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if(indices.Length == 0)
            {
                return null;
            }

            int index = -1;
            switch (indices.Max())
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    index = indices.Max();
                    break;
                case 7:
                case 8:
                    index = 7;
                    break;
                case 9:
                case 10:
                    index = 9;
                    break;
                case 11:
                case 12:
                    index = 11;
                    break;
                case 13:
                case 14:
                    index = 13;
                    break;
                default:
                    index = -1;
                    break;
            }

            return index == -1 ? null : KeywordService.Instance.POSITION_KEYWORD[index];
        }
    }
}
