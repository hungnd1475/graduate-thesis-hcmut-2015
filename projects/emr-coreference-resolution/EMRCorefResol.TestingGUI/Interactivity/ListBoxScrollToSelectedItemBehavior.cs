using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace EMRCorefResol.TestingGUI
{
    class ListBoxScrollToSelectedItemBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItem != null)
            {
                AssociatedObject.Dispatcher.BeginInvoke((Action)(() =>
                {
                    AssociatedObject.UpdateLayout();
                    if (AssociatedObject.SelectedItem != null)
                    {
                        AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
                    }
                }));
            }
        }
    }
}
