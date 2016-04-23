using HCMUT.EMRCorefResol;
using System.Collections.Generic;

namespace EMRCorefResol.TestingGUI
{
    static class ChainCollectionHelpers
    {
        public static CorefChain FindChainContains(this IEnumerable<CorefChain> chains, Concept concept, out int index)
        {
            index = -1;
            foreach (var ch in chains)
            {
                index += 1;
                if (ch.Contains(concept))
                {
                    return ch;
                }
            }

            index = -1;
            return null;
        }
    }
}
