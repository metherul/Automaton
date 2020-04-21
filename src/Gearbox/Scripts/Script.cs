using Gearbox.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Scripts
{
    public class Script
    {
        public static async Task<IRunnableScript> OpenManifest(string file)
        {
            var manifestFile = File.OpenRead(file);
            var manifest = await JsonUtils.ReadJson<ScriptManifest>(manifestFile);
            var scriptPath = Path.Combine(Path.GetDirectoryName(file), manifest.ScriptMain);

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Script file not found: {scriptPath}");
            }

            var script = manifest.ScriptType switch
            {
                ScriptType.Powershell => new PowershellScript(scriptPath),
                _ => throw new NotImplementedException($"Script type of {manifest.ScriptType.ToString()} has not been implemented.")
            };

            return script;
        }
    }
}
