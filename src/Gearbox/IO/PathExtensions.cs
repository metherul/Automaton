using System.IO;

namespace Gearbox.IO
{
    public class PathExtensions
    {
        public static string GetRelativePath(string baseFilePath, string parent)
        {
            return NormalizeFilePath(baseFilePath.Replace(parent, string.Empty));
        }

        public static string NormalizeFilePath(string filePath)
        {
            var original = filePath;

            // Check if the original starts with the dir separator char or a volume prefix.
            // If it does not, insert one. 
            if (original[1] != ':' && (original[0] != '/' && original[0] != '\\'))
            {
                original = original.Insert(0, Path.DirectorySeparatorChar.ToString());
            }

            // Generate a normalized path
            var almostNormal = Path.GetFullPath(original);

            // If the original does not start with a volume prefix, remove it 
            if (original[1] != ':')
            {
                // Remove "C:" (2 chars)
                almostNormal = almostNormal.Remove(0, 2);
            }

            // If it ends with a separator char, remove it.
            if (almostNormal[almostNormal.Length - 1] == Path.DirectorySeparatorChar)
            {
                almostNormal = almostNormal.Remove(almostNormal.Length - 1, 1);
            }

            return almostNormal;
        }
    }
}
