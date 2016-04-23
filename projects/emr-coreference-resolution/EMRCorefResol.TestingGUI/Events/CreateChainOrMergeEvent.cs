using HCMUT.EMRCorefResol;
using Prism.Events;
using System.Collections.Generic;

namespace EMRCorefResol.TestingGUI
{
    class CreateChainOrMergeEvent : PubSubEvent<IEnumerable<Concept>>
    {
    }
}
