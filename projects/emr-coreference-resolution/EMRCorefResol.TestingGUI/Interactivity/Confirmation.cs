using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;

namespace EMRCorefResol.TestingGUI
{
    public class Confirmation : IConfirmation, IInteractionRequestAware
    {
        public bool Confirmed { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
        public NotificationType NotificationType { get; set; }

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        public DelegateCommand ConfirmCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public Confirmation(NotificationType type)
        {
            NotificationType = type;

            ConfirmCommand = new DelegateCommand(Confirm);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            Confirmed = false;
            FinishInteraction();
        }

        private void Confirm()
        {
            Confirmed = true;
            FinishInteraction();
        }
    }
}
