using System.Collections.Generic;
using System.ComponentModel;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IMod : IModel
    {
        int CurrentDownloadPercentage { get; set; }
        string FileName { get; set; }
        string FilePath { get; set; }
        string FileSize { get; set; }
        List<IInstallation> InstallationParameters { get; set; }
        bool IsIndeterminateProcess { get; set; }
        string Md5 { get; set; }
        string ModInstallParameterPath { get; set; }
        string ModName { get; set; }
        string ModSourceUrl { get; set; }
        string NexusModId { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}