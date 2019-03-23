using System.Reflection;
using Automaton.Model.Interfaces;

namespace Automaton.Model
{
    public class ClassExtensions : IClassExtensions
    {
        public TDerived ToDerived<TBase, TDerived>(TBase tBase) where TDerived : TBase, new()
        {
            var tDerived = new TDerived();

            foreach (var propBase in typeof(TBase).GetProperties())
            {
                var propDerived = typeof(TDerived).GetProperty(propBase.Name);
                propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
            }

            return tDerived;
        }
    }
}
