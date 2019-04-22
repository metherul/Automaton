namespace Automaton.Model.Modpack
{
    public class ConfigPathOffsets
    {
        public static readonly string PackDefinitionConfig = "pack.auto_definition";
        public static readonly string PackThemeDefinition = "theme.auto_definition";
        public static readonly string PackInstallerDefinition = "installer.auto_definition";

        public static readonly string InstallConfig = "*/*/install.jsons";

        public static readonly string SettingsIni = "*.ini";
        public static readonly string MetaIni = "*/*/meta.ini";

        public static readonly string Plugins = "*/profile_metadata/plugins.txt";
        public static readonly string Modlist = "*/profile_metadata/modlist.txt";
        public static readonly string LockedOrder = "*/profile_metadata/lockedorder.txt";
        public static readonly string Archives = "*/profile_metadata/archives.txt";
    }
}
