using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class LoadingDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Message { get; set; }
        public string DebugText { get; set; }

        public int CurrentValue { get; set; }
        public int MaxValue { get; set; }

        public bool IsOpen { get; set; }

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
                DebugText = payload.DebugText;
            }

            if (payload.CurrentValue != null)
            {
                CurrentValue = (int)payload.CurrentValue;
            }

            if (payload.MaxValue != null)
            {
                MaxValue = (int)payload.MaxValue;
            }

            if (payload.IsOpen != null)
            {
                IsOpen = (bool)payload.IsOpen;

                if (IsOpen)
                {
                    OpenDialogHost();
                }
            }
        }

        private async void OpenDialogHost()
        {
            var view = new LoadingDialog();
            await DialogHost.Show(view, "RootDialog", OnDialogClose);
        }

        private void OnDialogClose(object sender, DialogClosingEventArgs eventArgs)
        {

        }
    }
}
