using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Gearbox.IO
{
    public class JsonUtils
    {
        public static async Task WriteJson<T>(T thing, string path)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(thing, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            var outputDir = Path.GetDirectoryName(path);

            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<T> ReadJson<T>(string path)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(File.ReadAllText(path)));
        }

        public static async Task<T> ReadJson<T>(Stream stream)
        {
            var item = await new StreamReader(stream).ReadToEndAsync();
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(item));
        }
    }
}
