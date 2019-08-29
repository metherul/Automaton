using Automaton.Common.Model;
using Automaton.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Automaton.Model
{
    public class LifetimeData : ILifetimeData
    {
        public MasterDefinition MasterDefinition { get; set; }
        public Manager ManagerDefinition { get; set; }
        public List<Mod> Mods { get; set; }
        public List<ExtendedArchive> Archives { get; set; }
        public List<ModpackItem> ModpackContent { get; set; }
        public string UserAgent { get; set; }
        public string InstallPath { get; set; }
        public string DownloadPath { get; set; }
        public string ApiKey { get; set; }
        public int CurrentDownloads { get; set; }

        public LifetimeData()
        {

            var platformType = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            UserAgent = $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; " +
                                $"{platformType}) {RuntimeInformation.FrameworkDescription}";
        }
    }
}
