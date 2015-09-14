using LAIR.ResourceAPIs.WordNet;

namespace HCMUT.EMRCorefResol.English
{
    public sealed class WordnetHelper
    {
        private static readonly WordNetEngine wEngine = new WordNetEngine(@"../../libs/wordnet/wordnetdb", true);

        static WordnetHelper()
        {
        }

        public static WordNetEngine Instance
        {
            get
            {
                return wEngine;
            }
        }
    }
}
