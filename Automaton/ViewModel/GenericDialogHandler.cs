using GalaSoft.MvvmLight.Messaging;

namespace Automaton.ViewModel
{
    class GenericDialogHandler
    {
        public static void OpenDialog(string title, string message)
        {
            var payload = new GenericDialogPayload()
            {
                Title = title,
                Message = message
            };

            Messenger.Default.Send(payload);
        }
    }

    class GenericDialogPayload
    {
        public string Title;
        public string Message;
    }
}
