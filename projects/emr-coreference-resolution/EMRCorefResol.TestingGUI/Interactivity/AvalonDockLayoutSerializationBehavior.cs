using EMRCorefResol.TestingGUI.Properties;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace EMRCorefResol.TestingGUI
{
    class AvalonDockLayoutSerializationBehavior : Behavior<DockingManager>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            base.OnDetaching();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(AssociatedObject);
            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream);
                Settings.Default.DockingLayout = stream.ToString();
                Settings.Default.Save();
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //var serializer = new XmlLayoutSerializer(AssociatedObject);
            //var savedLayout = Settings.Default.DockingLayout;
            //if (!string.IsNullOrEmpty(savedLayout))
            //{
            //    using (var stream = new StringReader(savedLayout))
            //    {
            //        serializer.Deserialize(stream);
            //    }
            //}
        }
    }
}
