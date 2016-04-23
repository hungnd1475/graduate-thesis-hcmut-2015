using HungND.WPF.Controls;
using Prism.Interactivity;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    class PopupVSWindowAction : PopupWindowAction
    { 
        protected override Window CreateWindow()
        {
            var window = new VSWindow()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                VSChrome = null,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                IsModal = IsModal
            };

            window.Loaded += Window_Loaded;
            return window;
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var w = (Window)sender;
            w.DisableCloseButton();
        }
    }
}
