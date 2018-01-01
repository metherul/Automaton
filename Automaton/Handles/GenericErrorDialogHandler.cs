using GalaSoft.MvvmLight.Messaging;

namespace Automaton.Handles
{
    class GenericErrorDialogHandler
    {
        public static void OpenDialog(string title, string errorMessage)
        {
            var payload = new GenericErrorDialogPayload()
            {
                Title = title,
                ErrorMessage = errorMessage
            };

            Messenger.Default.Send(payload);
        }
    }

    class GenericErrorDialogPayload
    {
        public string Title;
        public string ErrorMessage;
    }
}
