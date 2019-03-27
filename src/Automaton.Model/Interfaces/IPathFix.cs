namespace Automaton.Model.Interfaces
{
    public interface IPathFix : IModel
    {
        bool EnableLongPaths();
        bool IsLongPathsEnabled();
    }
}