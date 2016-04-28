using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for EMRContentView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class EMRContentView : DockableView
    {
        public static readonly string ID = typeof(EMRContentView).FullName;

        public EMRContentView()
            : base(DockableType.Document, ID)
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EMRContentView(EMRContentViewModel emrContentVM)
            : this()
        {
            DataContext = emrContentVM;
        }
    }
}
