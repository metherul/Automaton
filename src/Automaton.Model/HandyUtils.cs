using Automaton.Model.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Automaton.Model
{
    public class HandyUtils : IHandyUtils
    {
        public string GetMd5FromFile(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(fileStream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
