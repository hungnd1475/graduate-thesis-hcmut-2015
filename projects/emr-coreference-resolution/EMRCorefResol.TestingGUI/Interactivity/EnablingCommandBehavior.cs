using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace EMRCorefResol.TestingGUI
{
    class EnablingCommandBehavior : Behavior<FrameworkElement>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EnablingCommandBehavior), 
                new PropertyMetadata(null, CommandChangedCallback));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EnablingCommandBehavior), new PropertyMetadata(null));

        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as EnablingCommandBehavior;

            if (b != null)
            {
                var oldCommand = e.OldValue as ICommand;
                if (oldCommand != null)
                {
                    oldCommand.CanExecuteChanged -= (_, __) => Command_CanExecuteChanged(b);
                }

                var newCommand = e.NewValue as ICommand;
                if (newCommand != null)
                {
                    b.AssociatedObject.IsEnabled = newCommand.CanExecute(b.CommandParameter);
                    newCommand.CanExecuteChanged += (_, __) => Command_CanExecuteChanged(b);
                }
            }
        }

        private static void Command_CanExecuteChanged(EnablingCommandBehavior b)
        {
            if (b != null)
            {
                b.AssociatedObject.IsEnabled = b.Command.CanExecute(b.CommandParameter);
            }
        }
    }
}
