namespace Automaton.ViewModel.Dialogs.Interfaces
{
    public interface IGenericLogDialog : IDialog
    {
        string Message { get; set; }

        void DisplayParams(string message);
    }
}