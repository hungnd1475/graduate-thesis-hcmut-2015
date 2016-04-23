using EMRCorefResol.TestingGUI.Properties;
using HungND.WPF.Controls;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public MainWindow(MainViewModel mainVM)
            : this()
        {
            DataContext = mainVM;
        }

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            
        }

        private void IntegerUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var c = (IntegerUpDown)sender;
                var b = BindingOperations.GetBindingExpression(c, IntegerUpDown.ValueProperty);
                b.UpdateSource();
            }
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(DockingManager);
            var savedLayout = Settings.Default.DockingLayout;
            if (!string.IsNullOrEmpty(savedLayout))
            {
                using (var stream = new StringReader(savedLayout))
                {
                    serializer.Deserialize(stream);
                }
            }
        }
    }
}
