using LAIR.ResourceAPIs.WordNet;

namespace HCMUT.EMRCorefResol.English
{
    public sealed class WordnetHelper
    {
        private static readonly WordNetEngine wEngine = new WordNetEngine(@"../../../libs/wordnet/wordnetdb", true);

        static WordnetHelper()
        {
        }

        /// <summary>
        /// Create singleton for WordNetEngine
        /// Usage:
        ///     WordNetEngine wE = WordnetHelper.Instance;
        ///     var syns = wE.GetSynSets("...", WordNetEngine.POS...);
        /// </summary>
        public static WordNetEngine Instance
        {
            get
            {
                return wEngine;
            }
        }
    }
}
