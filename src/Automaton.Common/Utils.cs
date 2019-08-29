using IniParser;
using Newtonsoft.Json;
using SevenZipExtractor;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Automaton.Common
{
    public class Utils
    {
        public static T LoadJson<T>(string filename)
        {
            using (var s = File.OpenRead(filename))
            {
                return LoadJson<T>(s);
            }
        }

        public static T LoadJson<T>(Stream s)
        {
            using (var sr = new StreamReader(s))
            {
                return LoadJson<T>(sr);
            }
        }

        public static T LoadJson<T>(StreamReader sr)
        {
            var test1 = sr.ReadToEnd();
            //sr.BaseStream.Seek(0, SeekOrigin.Begin);
            var test = JsonConvert.DeserializeObject<T>(test1);

            return test;
        }

        public static T LoadJson<T>(Entry entry)
        {
            var entryStream = new MemoryStream();

            entry.Extract(entryStream);
            entryStream.Seek(0, SeekOrigin.Begin);

            return LoadJson<T>(entryStream);
        }

        public static void WriteJson<T>(T inst, string filename)
        {
            using (var os = File.OpenWrite(filename))
                WriteJson(inst, os);
        }

        private static void WriteJson<T>(T inst, Stream os)
        {
            using (var wtr = new StreamWriter(os))
                WriteJson(inst, wtr);
        }

        private static JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings() { Formatting = Formatting.Indented };

        private static void WriteJson<T>(T inst, StreamWriter wtr)
        {
            wtr.Write(JsonConvert.SerializeObject(inst, JSON_SETTINGS));
            wtr.Flush();
        }

        public static long FileSize(string fullPath)
        {
            return (new FileInfo(fullPath)).Length;
        }

        public static dynamic LoadIni(string filename)
        {
            var fi = new FileIniDataParser();
            return new DynamicIniData(fi.ReadFile(filename));
        }

        public static string FileSHA256(string filename, bool use_caching = false)
        {
            var sha_path = filename + ".sha256_hash";

            if (use_caching && File.Exists(sha_path) && new FileInfo(filename).LastWriteTime <= new FileInfo(sha_path).LastWriteTime)
                return Slurp(sha_path);

            using (var stream = File.OpenRead(filename))
            {
                var sha = new SHA256Managed();
                var hash = ToHex(sha.ComputeHash(stream));
                if (use_caching)
                    File.WriteAllText(sha_path, hash);
                return hash;
            }
        }


        public static string FileMD5(string filePath)
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

        public static string ToHex(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("x2"));

            return result.ToString();
        }

        public static void MemberwiseCopy<T1, T2>(T1 src, T2 dest)
        {
            foreach (var property in typeof(T1).GetProperties())
            {
                if (property.CanRead) {
                    var val = property.GetValue(src);
                    var destprop = typeof(T2).GetProperty(property.Name);
                    if (destprop != null && destprop.CanWrite)
                    {
                        destprop.SetValue(dest, val);
                    }
                }
            }
        }

        /// <summary>
        /// Strips prefix.Length charaters from the start of value
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripPrefix(string prefix, string value)
        {
            if (prefix.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return value.Substring(prefix.Length);
            return value.Substring(prefix.Length + 1);
        }

        public static string Slurp(Stream s)
        {
            using (var rdr = new StreamReader(s))
            {
                return rdr.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads in a complete text file and returns the contents as a string
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Slurp(string file)
        {
            using (var rdr = new StreamReader(File.OpenRead(file)))
            {
                return rdr.ReadToEnd();
            }
        }

        /// <summary>
        /// Writes a object as JSON into a entry with the given name in the given zip file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="zip"></param>
        /// <param name="entry_name"></param>
        /// <param name="json_data"></param>
        public static void SpitJsonInto<T>(ZipArchive zip, string entry_name, T json_data)
        {
            var entry = zip.CreateEntry(entry_name);
            using (var wtr = new StreamWriter(entry.Open()))
            {
                WriteJson(json_data, wtr);
            }
        }

        public static void SpitInto(ZipArchive zip, string entry_name, string data)
        {
            var entry = zip.CreateEntry(entry_name);
            using (var wtr = new StreamWriter(entry.Open()))
            {
                wtr.Write(data);
            }
        }

        public static MemoryStream GetEntryMemoryStream(Entry entry)
        {
            var memoryStream = new MemoryStream();
            entry.Extract(memoryStream);

            return memoryStream;
        }

        public static string SHA256(Stream result, string cache_name = null)
        {
            var cache_file = cache_name == null ? null : Path.Combine("stream_caches", cache_name);
            if (cache_file != null && File.Exists(cache_file))
                return File.ReadAllText(cache_file);

            using (var stream = result)
            {
                var sha = new SHA256Managed();
                var hash = ToHex(sha.ComputeHash(stream));

                if (cache_file != null)
                {
                    Directory.CreateDirectory("stream_caches");
                    File.WriteAllText(cache_file, hash);
                }

                return hash;
            }
        }

        public static string GetNexusAPIKey()
        {
            FileInfo fi = new FileInfo("nexus.key_cache");
            if (fi.Exists && fi.LastWriteTime > DateTime.Now.AddHours(-12))
            {
                return Utils.Slurp("nexus.key_cache");
            }

            var guid = Guid.NewGuid();
            var _websocket = new WebSocket("wss://sso.nexusmods.com")
            {
                SslConfiguration =
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            }
            };

            TaskCompletionSource<string> api_key = new TaskCompletionSource<string>();
            _websocket.OnMessage += (sender, msg) =>
            {
                api_key.SetResult(msg.Data);
                return;
            };

            _websocket.Connect();
            _websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

            Process.Start($"https://www.nexusmods.com/sso?id={guid}&application=Automaton");

            api_key.Task.Wait();
            var result = api_key.Task.Result;
            File.WriteAllText("nexus.key_cache", result);
            return result;
        }
    }
}
