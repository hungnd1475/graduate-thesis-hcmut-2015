using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    public class AdjacentMentionAfterPronounVP : Feature
    {
        public AdjacentMentionAfterPronounVP(PronounInstance instance, EMR emr)
            : base("AdjacentMention-AfterPronounVP", 2, 0)
        {
            var line = emr.GetLine(instance.Concept);

            var posTag = Service.English.POSTag(line);
            var chunks = Service.English.GetChunks(line);

            if(CheckFormat(line, instance.Concept, posTag, chunks))
            {
                SetCategoricalValue(1);
            }
        }

        private bool CheckFormat(string line, Concept c, string[] posTag, string[] chunks)
        {
            var termIndex = GetConceptIndex(c, posTag);
            if(termIndex <= 1)
            {
                return false;
            }

            var chunkBeforeMention = GetChunkBeforeMention(termIndex, chunks);

            //Chunks before is not a verb phrase
            if(!chunkBeforeMention.Item1.Equals("VP") ||
                chunkBeforeMention.Item2 == 0)
            {
                return false;
            }

            var posTagBeforeChunk = GetPosTag(posTag[chunkBeforeMention.Item2 - 1]);
            if(!posTagBeforeChunk.Equals("PRP") &&
                !posTagBeforeChunk.Equals("PRP$") &&
                !posTagBeforeChunk.Equals("WP") &&
                !posTagBeforeChunk.Equals("WP$"))
            {
                return false;
            }

            return true;
        }

        private Tuple<string, int> GetChunkBeforeMention(int conceptIndex, string[] chunks)
        {
            var currentIndex = conceptIndex - 1;
            var chunkBegin = 0;
            var tag = "";

            while(currentIndex >= 0)
            {
                var chunkTag = GetChunkTag(chunks[currentIndex]);
                if(tag.Equals("") || tag.Equals(chunkTag))
                {
                    tag = chunkTag;
                    chunkBegin = currentIndex;
                } else
                {
                    break;
                }

                currentIndex--;
            }

            return Tuple.Create(tag, chunkBegin);
        }

        private int GetConceptIndex(Concept c, string[] postTag)
        {
            int start = 0;
            if (c.Begin.WordIndex == 0)
            {
                return 0;
            } else if(c.Begin.WordIndex < 3)
            {
                start = 0;
            } else
            {
                start = c.Begin.WordIndex - 3;
            }

            var end = 0;
            if (start + 6 < postTag.Length - 1)
            {
                end = start + 6;
            }
            else
            {
                end = postTag.Length - 1;
            }

            for (int i=start; i<end; i++)
            {
                var tag = postTag[i];
                var term = GetTerm(tag);
                if (term.Equals(c.Lexicon))
                {
                    return i;
                }
            }

            return c.Begin.WordIndex;
        }

        private string GetTerm(string tag)
        {
            return tag.Split('|')[0];
        }

        private string GetPosTag(string tag)
        {
            return tag.Split('|')[1];
        }

        private string GetChunkTag(string tag)
        {
            var chunk = tag.Split('|')[1];
            if (chunk.Contains("-"))
            {
                return chunk.Split('-')[1];
            } else
            {
                return chunk;
            }
        }
    }
}
