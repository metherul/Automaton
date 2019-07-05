namespace Automaton.Model.Interfaces
{
    public interface ILoadModpack : IModel
    {
        void Load(string modpackPath);
        System.Threading.Tasks.Task LoadAsync(string modpackPath);
    }
}