namespace Automaton.Model.Modpack
{
    public class ConfigPathOffsets
    {
        public static readonly string PackDefinitionConfig = "pack.auto_definition";
        public static readonly string InstallConfig = "mods/*/install.json";

        public static readonly string SettingsIni = "*.ini";
        public static readonly string MetaIni = "mods/*/meta.ini";

        public static readonly string Plugins = "plugins.txt";
        public static readonly string Modlist = "modlist.txt";
        public static readonly string LockedOrder = "lockedorder.txt";
        public static readonly string Archives = "archives.txt";
    }
}
