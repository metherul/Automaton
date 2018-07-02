using System;
using System.IO;
using System.Security.Cryptography;

namespace Automaton.Model.Handles
{
    public class Md5Handle
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
