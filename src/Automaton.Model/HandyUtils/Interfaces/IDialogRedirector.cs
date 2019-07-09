using Automaton.Model.Interfaces;

namespace Automaton.Model.HandyUtils.Interfaces
{
    public interface IDialogRedirector : ISingleton
    {
        event DialogRedirector.RoutedLog RoutedLogEvent;

        void RouteLog(string message);
    }
}