using System.IO;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks.OMS.Base;

namespace Gearbox.Modpacks.OMS
{
    public class Header : IHeader, IJsonSource
    {
        public string Name => _headerBase?.Name;
        public string Author => _headerBase?.Author;
        public string Version => _headerBase?.Version;

        private HeaderBase _headerBase;
        
        public async Task FromJson(Stream stream)
        {
            _headerBase = await JsonUtils.ReadJson<HeaderBase>(stream);
        }
    }
}