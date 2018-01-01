using GalaSoft.MvvmLight.Messaging;

namespace Automaton.ViewModel
{
    class LoadingDialogHandle
    {
        public static void OpenDialog(string title, string message, int maxValue)
        {
            var payload = new LoadingDialogPayload()
            {
                Title = title,
                Message = message,
                DebugText = "",
                MaxValue = maxValue,
                CurrentValue = 0,
                IsOpen = true
            };

            SendPayload(payload);
        }

        public static void UpdateDialog(string title, string message, string debugText, int currentValue)
        {

        }

        public static void UpdateDebugText(string debugText)
        {

        }

        public static void UpdateMessage(string message)
        {

        }

        private static void SendPayload(LoadingDialogPayload payload)
        {
            Messenger.Default.Send(payload);
        }
    }

    class LoadingDialogPayload
    {
        public string Title;
        public string Message;
        public string DebugText;
        public int? MaxValue;
        public int? CurrentValue;
        public bool? IsOpen;
    }
}
