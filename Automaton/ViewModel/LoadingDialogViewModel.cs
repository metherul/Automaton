using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class LoadingDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Message { get; set; }
        public string DebugText { get; set; }

        public bool IsOpen { get; set; }
        public bool IsDoneLoading { get; set; }

        public LoadingDialogViewModel()
        {
            Messenger.Default.Register<LoadingDialogPayload>(this, RecievePayload);
        }

        private void RecievePayload(LoadingDialogPayload payload)
        {
            if (!string.IsNullOrEmpty(payload.Title))
            {
                Title = payload.Title;
            }

            if (!string.IsNullOrEmpty(payload.Message))
            {
                Message = payload.Message;
            }

            if (!string.IsNullOrEmpty(payload.DebugText))
            {
                DebugText += $"{payload.DebugText} \n";
            }

            if (payload.IsOpen != null)
            {
                IsOpen = (bool)payload.IsOpen;
            }

            if (payload.IsDoneLoading != null)
            {
                IsDoneLoading = (bool)payload.IsDoneLoading;
            }
        }
    }
}
