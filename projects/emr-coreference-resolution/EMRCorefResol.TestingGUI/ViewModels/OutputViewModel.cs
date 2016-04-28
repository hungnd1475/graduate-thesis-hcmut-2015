using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OutputViewModel : BindableBase
    {
        private readonly StringBuilder _outputBuilder;

        private string _outputText;
        public string OutputText
        {
            get { return _outputText; }
            set { SetProperty(ref _outputText, value); }
        }

        public DelegateCommand ClearAllCommand { get; }

        [ImportingConstructor]
        public OutputViewModel(IEventAggregator eventAggregator)
        {
            _outputBuilder = new StringBuilder();
            eventAggregator.GetEvent<OutputEvent>().Subscribe(OnOutput, ThreadOption.UIThread);
        }

        private void OnOutput(string text)
        {
            _outputBuilder.AppendLine(text);
            OutputText = _outputBuilder.ToString();
        }
    }
}
