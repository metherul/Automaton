using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IHeader : IModel
    {
        string AutomatonVersion { get; set; }
        string BackgroundColor { get; set; }
        string ButtonColor { get; set; }
        bool ContainsSetupAssistant { get; set; }
        string ControlColor { get; set; }
        string Description { get; set; }
        string FontColor { get; set; }
        string HeaderImage { get; set; }
        bool InstallModOrganizer { get; set; }
        List<string> ModInstallFolders { get; set; }
        int ModOrganizerVersion { get; set; }
        string ModpackAuthor { get; set; }
        string ModpackName { get; set; }
        string ModpackSourceUrl { get; set; }
        string ModpackVersion { get; set; }
        ISetupAssistant SetupAssistant { get; set; }
        string TargetGame { get; set; }
    }
}