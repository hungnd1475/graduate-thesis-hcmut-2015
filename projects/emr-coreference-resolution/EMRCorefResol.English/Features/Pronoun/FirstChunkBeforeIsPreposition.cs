using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    /// <summary>
    /// Set value = 1 if 1st chunk before is preposition (PP in chunking)
    /// </summary>
    class FirstChunkBeforeIsPreposition : Feature
    {
        public FirstChunkBeforeIsPreposition(FirstChunkBeforeMention feature)
            :base("FirstChunkBefore-IsPreposition", 2, 0)
        {
            //If chunk before is PP (preposition phrase)
            if(feature.GetCategoricalValue() == 2)
            {
                SetCategoricalValue(1);
            }
        }

        public FirstChunkBeforeIsPreposition(PronounInstance instance, EMR emr)
            : base("FirstChunkBefore-IsPreposition", 2, 0)
        {
            var line = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line);

            var chunks = Service.English.GetChunks(line);

            if (instance.Concept.Begin.WordIndex > 0)
            {
                var previousChunk = chunks[instance.Concept.Begin.WordIndex - 1];
                var tags = previousChunk.Split('|')[1].Split('-');
                if (!tags[0].Equals("O", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(tags[1].Equals("PP", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }
        }
    }
}
