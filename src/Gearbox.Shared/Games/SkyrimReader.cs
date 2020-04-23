using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Shared.Games
{
    public class SkyrimReader : IGameReader
    {
        public bool IsGameExe(string exe)
        {
            return Path.GetFileName(exe) == "Skyrim.exe";
        }

        public Task<List<string>> GetGameFiles()
        {
            throw new NotImplementedException();
        }
    }
}
