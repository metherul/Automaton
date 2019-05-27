namespace Automaton.Model.Interfaces
{
    public interface IFileDownload
    {
        IFileDownload New(string downloadUrl);
        void Download();
    }
}
