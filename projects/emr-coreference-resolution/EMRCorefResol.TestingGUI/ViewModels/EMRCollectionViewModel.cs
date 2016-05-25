using HCMUT.EMRCorefResol;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EMRCollectionViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private EMRCollection _emrCollection;
        private bool onEMRChanged = false;

        private List<EMRItem> _emrItems;
        public List<EMRItem> EMRItems
        {
            get { return _emrItems; }
            set { SetProperty(ref _emrItems, value); }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (SetProperty(ref _selectedIndex, value) && !onEMRChanged)
                {
                    _eventAggregator.GetEvent<EMRIndexChangedEvent>().Publish(value);
                }
            }
        }

        [ImportingConstructor]
        public EMRCollectionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRCollectionChangedEvent>().Subscribe(OnEMRCollectionChanged, ThreadOption.BackgroundThread);
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(OnEMRChanged, ThreadOption.BackgroundThread);
        }

        private void OnEMRChanged(EMRChangedEventArgs e)
        {
            int index = -1;
            if (e?.EMR != null && _emrCollection != null)
            {
                index = _emrCollection.IndexOf(e.EMR.GetEMRName());
            }

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                onEMRChanged = true;
                SelectedIndex = index;
                onEMRChanged = false;
            }));
        }

        private void OnEMRCollectionChanged(EMRCollection emrCollection)
        {
            List<EMRItem> items = null;
            if ((_emrCollection = emrCollection) != null)
            {
                items = new List<EMRItem>(_emrCollection.Count);
                for (int i = 0; i < _emrCollection.Count; i++)
                {
                    var path = _emrCollection.GetEMRPath(i);
                    var name = Path.GetFileName(path);
                    items.Add(new EMRItem(i, name));
                }
            }

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                EMRItems = items;
                SelectedIndex = -1;
            }));
        }
    }
}
