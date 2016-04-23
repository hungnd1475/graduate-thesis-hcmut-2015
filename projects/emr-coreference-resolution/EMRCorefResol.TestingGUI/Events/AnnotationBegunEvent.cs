using HCMUT.EMRCorefResol;
using Prism.Events;
using System.Collections.ObjectModel;

namespace EMRCorefResol.TestingGUI
{
    class AnnotationBegunEvent : PubSubEvent<ObservableCollection<CorefChain>>
    {
    }
}
