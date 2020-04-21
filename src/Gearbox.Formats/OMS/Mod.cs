using System;

namespace Gearbox.Formats.OMS
{
    public class Mod
    {
        public string Name { get; set; }
        public ModType ModType { get; set; }
        public string[] RequiredArchives { get; set; }
        public Set[] Install { get; set; }
    }
}