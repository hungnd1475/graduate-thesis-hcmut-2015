using EMRCorefResol.TestingGUI.Properties;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.Linq;
using System.Collections.Specialized;

namespace EMRCorefResol.TestingGUI
{
    class AvalonDockLayoutSerializationBehavior : Behavior<DockingManager>
    {
        public string SavedLayoutSettingName { get; set; }
        public string LastViewsSettingName { get; set; }
        public IAvalonDockViewsRegistry ViewsRegistry { get; set; }

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

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewsRegistry != null)
            {
                ViewsRegistry.RegisterViews(AssociatedObject);

                var serializer = new XmlLayoutSerializer(AssociatedObject);
                var savedLayout = Settings.Default[SavedLayoutSettingName] as string;
                if (!string.IsNullOrEmpty(savedLayout))
                {
                    using (var sr = new StringReader(savedLayout))
                    {
                        serializer.Deserialize(sr);
                    }
                }
            }
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(AssociatedObject);
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw);
                Settings.Default[SavedLayoutSettingName] = sw.ToString();
            }

            var openedViewTypes = new StringCollection();
            foreach (var view in AssociatedObject.DocumentsSource)
            {
                openedViewTypes.Add(view.GetType().AssemblyQualifiedName);
            }
            foreach (var view in AssociatedObject.AnchorablesSource)
            {
                openedViewTypes.Add(view.GetType().AssemblyQualifiedName);
            }
            Settings.Default[LastViewsSettingName] = openedViewTypes;
            Settings.Default.Save();
        }
    }
}
