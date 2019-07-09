using Automaton.Model.HandyUtils.Interfaces;
using Automaton.Model.Interfaces;

namespace Automaton.Model.HandyUtils
{
    public class DialogRedirector : IDialogRedirector
    {
        public delegate void RoutedLog(string message);
        public event RoutedLog RoutedLogEvent;

        public void RouteLog(string message)
        {
            RoutedLogEvent.Invoke(message);
        }
    }
}
