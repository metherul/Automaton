using IniParser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Automaton.Utils
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
            return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
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

        public static string FileSHA256(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var sha = new SHA256Managed();
                return ToHex(sha.ComputeHash(stream));
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
    }
}
