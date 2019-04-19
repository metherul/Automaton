using Newtonsoft.Json;
using System;
using System.IO;

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
    }
}
