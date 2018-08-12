using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Automaton.Model.Utility
{
    public class Md5
    {
        public static string CalculateMd5(string filePath)
        {
            var md5Hash = "";

            using (var stream = File.OpenRead(filePath))
            {
                var byteHash = MD5.Create().ComputeHash(stream);

                md5Hash = BitConverter.ToString(byteHash).Replace("-", "").ToLowerInvariant();
            }

            return md5Hash;
        }
    }
}
