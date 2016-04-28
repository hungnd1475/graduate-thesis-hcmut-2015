using HCMUT.EMRCorefResol;
using Prism.Events;
using System.Collections.ObjectModel;

namespace EMRCorefResol.TestingGUI
{
    class CorefAnnotationBegunEvent : PubSubEvent<ObservableCollection<CorefChain>>
    {
    }
}
