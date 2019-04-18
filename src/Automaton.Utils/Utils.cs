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
    }
}
