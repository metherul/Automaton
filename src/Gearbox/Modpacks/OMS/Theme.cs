using System.IO;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks.OMS.Base;

namespace Gearbox.Modpacks.OMS
{
    public class Theme : ITheme, IJsonSource
    {
        public string ThemeColor => _themeBase?.ThemeColor;
        public string Background => _themeBase?.Background;
        public string TextForeground => _themeBase?.TextForeground;
        public string ButtonForeground => _themeBase?.ButtonForeground;
        public string ButtonBackground => _themeBase?.ButtonBackground;

        private ThemeBase _themeBase;
        
        public async Task FromJson(Stream stream)
        {
            _themeBase = await JsonUtils.ReadJson<ThemeBase>(stream);
        }
    }
}