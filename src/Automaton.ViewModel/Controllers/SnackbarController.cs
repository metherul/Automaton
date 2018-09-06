using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.ViewModel.Controllers
{
    public class SnackbarController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static string Message { get; set; }
        public static bool IsActive { get; set; }

        public static void EnqueueMessage(string message)
        {
            IsActive = true;

            Task.Factory.StartNew(() =>
            {
                Message = message;

                Thread.Sleep(1500);
            }).Wait();

            IsActive = false;
        }
    }
}
