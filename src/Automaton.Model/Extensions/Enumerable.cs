using System.Collections.Generic;
using System.Linq;

namespace Automaton.Model.Extensions
{
    public static class Enumerable
    {
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }
    }
}