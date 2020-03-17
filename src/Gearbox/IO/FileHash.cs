using Force.Crc32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Gearbox.IO
{
    public class FileHash
    {
        public static string GetMd5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public static async Task<string> GetMd5Async(Stream stream)
        {
            return await Task.Run(() => GetMd5(stream));
        }

        public static async Task<uint> GetCrcAsync(Stream stream)
        {
//            using var stream = File.OpenRead(filePath);
            using var crc = new Crc32Algorithm();
            var buffer = await Task.Run(() => crc.ComputeHash(stream));

            return BitConverter.ToUInt32(buffer, 0);
        }
    }
}
