using System;
using System.IO;
using System.Threading.Tasks;
using Gearbox.Modpacks.OMS;

namespace Gearbox.Modpacks
{
    public class PackFactory
    {
        public async static Task<IPack> FromFile(string filePath)
        {
            var pack = new Pack();
            
            if (Path.GetExtension(filePath).ToLower() == "wabbajack")
            {
                throw new NotImplementedException();
            }

            PackResources.Pack = pack;
            await pack.FromFile(filePath);
            
            return pack;
        }
    }
}