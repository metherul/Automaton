namespace Automaton.Model.Interfaces
{
    public interface IClassExtensions : IModel
    {
        TDerived ToDerived<TBase, TDerived>(TBase tBase) where TDerived : TBase, new();
    }
}