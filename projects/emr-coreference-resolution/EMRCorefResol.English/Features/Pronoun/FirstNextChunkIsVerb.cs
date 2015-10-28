using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    /// <summary>
    /// Set value = 1 if 1st next chunk is verb phrase (VP in chunking)
    /// </summary>
    class FirstNextChunkIsVerb : Feature
    {
        public FirstNextChunkIsVerb(FirstChunkAfterMention feature)
            :base("FirstNextChunk-IsVerb", 2, 0)
        {
            if(feature.GetCategoricalValue() == 3)
            {
                SetCategoricalValue(1);
            }
        }

        public FirstNextChunkIsVerb(PersonInstance instance, EMR emr)
            :base("FirstNextChunk-IsVerb", 2, 0)
        {
            var line = EMRExtensions.GetLine(emr, instance.Concept);

            var chunks = Service.English.GetChunks(line);

            var endIndex = instance.Concept.End.WordIndex;
            if(instance.Concept.Begin.Line != instance.Concept.End.Line)
            {
                var beginLine = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line);
                var beginMaxWord = beginLine.Replace("\r", "").Replace("\n", "").Split(' ').Length;
                endIndex = beginMaxWord + instance.Concept.End.WordIndex;
            }

            if (endIndex < chunks.Length - 1)
            {
                var nextChunk = chunks[endIndex + 1];
                var tags = nextChunk.Split('|')[1].Split('-');
                if (!tags[0].Equals("O", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (tags[1].Equals("VP", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }
        }
    }
}
