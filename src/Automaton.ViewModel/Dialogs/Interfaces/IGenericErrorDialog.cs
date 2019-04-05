namespace Automaton.ViewModel.Content.Dialogs.Interfaces
{
    public interface IGenericErrorDialog : IDialog
    {
        string ErrorHeader { get; set; }
        string ErrorMessage { get; set; }
        bool IsFatal { get; set; }
        void DisplayParams(bool isFatal, string header, string message);
    }
}