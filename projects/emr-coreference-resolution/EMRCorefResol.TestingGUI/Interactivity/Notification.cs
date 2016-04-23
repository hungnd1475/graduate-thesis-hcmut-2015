using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;

namespace EMRCorefResol.TestingGUI
{
    public class Notification : BindableBase, INotification, IInteractionRequestAware
    {
        public string Title { get; set; }
        public object Content { get; set; }
        public NotificationType NotificationType { get; }

        INotification IInteractionRequestAware.Notification { get; set; }
        public Action FinishInteraction { get; set; }

        public DelegateCommand ReturnCommand { get; }

        public Notification(NotificationType type)
        {
            NotificationType = type;
            ReturnCommand = new DelegateCommand(Return);
        }

        protected virtual void Return()
        {
            FinishInteraction();
        }
    }
}
