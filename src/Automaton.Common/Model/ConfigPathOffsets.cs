namespace Automaton.Common.Model
{
    public class ConfigPathOffsets
    {
        public static readonly string PackDefinitionConfig = "pack.auto_definition";
        public static readonly string InstallConfig = "install.json";

        public static readonly string DefaultContentDir = "content";
        public static readonly string DefaultModsDir = "mods";

        public static readonly string SettingsIni = "*.ini";
        public static readonly string MetaIni = "mods/*/meta.ini";

        public static readonly string Plugins = "plugins.txt";
        public static readonly string Modlist = "modlist.txt";
        public static readonly string LockedOrder = "lockedorder.txt";
        public static readonly string Archives = "archives.txt";
    }
}
