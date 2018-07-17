using System.IO;

namespace Automaton.Model.Extensions
{
    public static class Path
    {
        public static string StandardizePathSeparators(this string inputString)
        {
            var directorySeparator = System.IO.Path.DirectorySeparatorChar.ToString();

            if (!string.IsNullOrEmpty(inputString))
            {
                return inputString.Replace("/", directorySeparator);
            }

            return inputString;
        }
    }
}