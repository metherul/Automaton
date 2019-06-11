namespace Automaton.Model.Interfaces
{
    public interface IFileDownload : IModel
    {
        IFileDownload New(string downloadUrl);
        void Download();
    }
}
