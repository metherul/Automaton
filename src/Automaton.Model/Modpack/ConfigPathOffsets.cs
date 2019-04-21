namespace Automaton.Model.Modpack
{
    public class ConfigPathOffsets
    {
        public readonly string PackDefinitionConfig = "pack.auto_definition";
        public readonly string InstallConfig = "mods/{0}/install.json";

        public readonly string SettingsIni = "*.ini";
        public readonly string MetaIni = "mods/*/meta.ini";

        public readonly string Plugins = "plugins.txt";
        public readonly string Modlist = "modlist.txt";
        public readonly string LockedOrder = "lockedorder.txt";
        public readonly string Archives = "archives.txt";
    }
}
