using HCMUT.EMRCorefResol;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    class EntityAnnotationBegunEvent : PubSubEvent<ObservableCollection<Concept>>
    {
    }
}
