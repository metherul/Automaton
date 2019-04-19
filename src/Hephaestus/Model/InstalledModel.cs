using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Automaton.Utils;
using System.IO;
using IniParser;

namespace Hephaestus.Model
{
    public class InstalledMod
    {
        private IniData _ini;
        public string ModName { get; set; }
        public string FullPath { get; set; }
        public IniData Ini { get
            {
                return _ini;
            }
            set
            {
                _ini = value;
                MetaData = new DynamicIniData(value);
            }
        }
        public dynamic MetaData { get; set; }

        public static InstalledMod FromFolder(string path)
        {
            var mod = new InstalledMod();
            mod.ModName = Path.GetFileName(path);
            mod.FullPath = path;
            var parser = new FileIniDataParser();
            mod.Ini = parser.ReadFile(Path.Combine(path, "meta.ini"));
            return mod;
                
        }
    }
}
