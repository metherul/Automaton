namespace Automaton.Model.Interfaces
{
    public interface ILifetimeData : ISingleton
    {
        string UserAgent { get; set; }
        string InstallPath { get; set; }
        string DownloadPath { get; set; }
        string ApiKey { get; set; }
        Common.Model.MasterDefinition MasterDefinition { get; set; }
        System.Collections.Generic.List<Common.Model.Mod> Mods { get; set; }
        
        System.Collections.Generic.List<ModpackItem> ModpackContent { get; set; }
        System.Collections.Generic.List<ExtendedArchive> Archives { get; set; }
    }
}
