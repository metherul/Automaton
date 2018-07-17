using System;
using System.IO;
using System.Security.Cryptography;

namespace Automaton.Model.Utility
{
    public class MD5
    {
        public static string CalculateMd5(string filePath)
        {
            var md5Hash = "";

            using (var stream = File.OpenRead(filePath))
            {
                var byteHash = System.Security.Cryptography.MD5.Create().ComputeHash(stream);

                md5Hash = BitConverter.ToString(byteHash).Replace("-", "").ToLowerInvariant();
            }

            return md5Hash;
        }
    }
}
