namespace Automaton.ViewModel.Controllers.Interfaces
{
    public interface IDialogController : IController
    {
        int CurrentIndex { get; set; }
        bool IsDialogOpen { get; set; }
        void OpenErrorDialog(bool isFatal, string header, string message);
    }
}