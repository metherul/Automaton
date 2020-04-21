using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gearbox.Shared.JsonExt
{
    public class JsonExt
    {
        public static async Task WriteJson<T>(T thing, string path)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true
            };

            var outDir = Path.GetDirectoryName(path);
            
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            using var fileStream = File.Create(path);
            await JsonSerializer.SerializeAsync(fileStream, thing, options);
        }

        public static async Task<T> ReadJson<T>(string file)
        {
            using var fileStream = File.OpenRead(file);
            var thing = await JsonSerializer.DeserializeAsync<T>(fileStream);

            return thing;
        }
    }
}
