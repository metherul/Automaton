using System;
using System.Linq;

namespace Gearbox.Modpacks.Base
{
    public class MegaSource
    {
        public string Url;

        public MegaSource(string stuff)
        {
            var urlKey = "url";
            
            var lines = stuff.Split(Environment.NewLine);
            Url = lines
                .First(x => x.ToLower().StartsWith(urlKey))
                .Replace(" ", "")[(urlKey.Length + 2)..^1];
        }
    }
}