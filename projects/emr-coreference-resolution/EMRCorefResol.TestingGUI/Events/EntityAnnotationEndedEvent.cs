using HCMUT.EMRCorefResol;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    class EntityAnnotationEndedEvent : PubSubEvent<EntityAnnotationEndedEventArgs>
    {
    }

    class EntityAnnotationEndedEventArgs
    {
        public EMR ResultEMR { get; }
        public CorefChainCollection ResultChains { get; }

        public EntityAnnotationEndedEventArgs(EMR resultEMR,
            CorefChainCollection resultChains)
        {
            ResultEMR = resultEMR;
            ResultChains = resultChains;
        }
    }
}
