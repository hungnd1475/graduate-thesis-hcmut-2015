using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace EMRCorefResol.TestingGUI
{
    class UpdateSourceOnReturnBehavior : Behavior<FrameworkElement>
    {
        private Type _associatedType;

        public string BoundPropertyName
        {
            get { return (string)GetValue(BoundPropertyNameProperty); }
            set { SetValue(BoundPropertyNameProperty, value); }
        }
        
        public static readonly DependencyProperty BoundPropertyNameProperty =
            DependencyProperty.Register("BoundPropertyName", typeof(string), typeof(UpdateSourceOnReturnBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            _associatedType = AssociatedObject.GetType();
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
            base.OnDetaching();
        }

        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                var dpd = DependencyPropertyDescriptor.FromName(BoundPropertyName,
                    _associatedType, _associatedType);

                if (dpd != null && dpd.DependencyProperty != null)
                {
                    var be = BindingOperations.GetBindingExpression(AssociatedObject, dpd.DependencyProperty);
                    if (be != null)
                    {                 
                        try { be.UpdateSource(); }   
                        catch (Exception) { }                           
                    }
                }               
            }
        }
    }
}
