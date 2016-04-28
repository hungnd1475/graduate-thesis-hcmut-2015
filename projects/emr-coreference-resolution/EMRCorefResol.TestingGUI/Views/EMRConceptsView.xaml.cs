using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for EMRConceptsView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class EMRConceptsView
    {
        public static readonly string ID = typeof(EMRConceptsView).FullName;

        public EMRConceptsView()
            : base(DockableType.Document, ID)
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EMRConceptsView(EMRConceptsViewModel emrConceptsVM)
            : this()
        {
            DataContext = emrConceptsVM;
        }
    }
}
