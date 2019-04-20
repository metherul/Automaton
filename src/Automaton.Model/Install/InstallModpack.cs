using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Install.Interfaces;
using Automaton.Model.Interfaces;
using System;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Automaton.Model.Install
{
    public class InstallModpack : IInstallModpack
    {
        private readonly IInstallBase _installBase;
        private readonly IArchiveContents _archiveContents;
        private readonly ILogger _logger;
        private readonly ICommonFilesystemUtility _commonFilesystemUtility;
        private readonly IRegistryHandle _registryHandle;

        private List<ExtendedMod> _modList;

        public EventHandler<string> DebugLogCallback { get; set; }

        public InstallModpack(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _archiveContents = components.Resolve<IArchiveContents>();
            _logger = components.Resolve<ILogger>();
            _commonFilesystemUtility = components.Resolve<ICommonFilesystemUtility>();
            _registryHandle = components.Resolve<IRegistryHandle>();
        }

        public void Install()
        {
            _modList = _installBase.ModpackMods;

            // Install Mod Organizer
            var installPath = Path.Combine(_installBase.InstallDirectory, _installBase.ModpackHeader.Name);
            
            if (_installBase.ModpackHeader.InstallModOrganizer)
            {
                DebugWrite("[INSTALL] Installing Mod Organizer...");
                var modOrganizerObject = _modList.First(x => x.IsModOrganizer);

                _archiveContents.ExtractToDirectory(modOrganizerObject.FilePath, installPath);
                _modList.Remove(modOrganizerObject);

                var gamePath = _registryHandle.GetGamePath(_installBase.ModpackHeader.TargetGame ?? "Skyrim").Replace(@"\", @"\\");

                File.Create(Path.Combine(installPath, "ModOrganizer.ini")).Close();
                File.WriteAllText(Path.Combine(installPath, "ModOrganizer.ini"),
                    "[General] \n" +
                    $"gameName={_installBase.ModpackHeader.TargetGame ?? "Skyrim"} \n" +
                    $"gamePath={gamePath} \n" +
                    $"first_start=false"
                );

                Directory.CreateDirectory(Path.Combine(installPath), "mods");

                // Clear the registry if applicable
                _registryHandle.ClearMOCurrentInstance();

            }

            foreach (var mod in _modList)
            {
                InstallMod(mod);
            }

            // Write the needed profile information
            var profilePath = System.IO.Path.Combine(installPath, "profiles", _installBase.ModpackHeader.Name);

            if (Directory.Exists(profilePath))
            {
                _commonFilesystemUtility.DeleteDirectory(profilePath);
            }

            Directory.CreateDirectory(profilePath);

            File.WriteAllText(Path.Combine(profilePath, "plugins.txt"), _installBase.PluginsTxt);
            File.WriteAllText(Path.Combine(profilePath, "loadorder.txt"), _installBase.LoadorderTxt);
            File.WriteAllText(Path.Combine(profilePath, "modlist.txt"), _installBase.ModlistTxt);
            File.WriteAllText(Path.Combine(profilePath, "archives.txt"), _installBase.ArchivesTxt);

            DebugWrite("_END");
        }

        public void InstallMod(ExtendedMod mod)
        {
            DebugWrite($"[INSTALL] {mod.ModName}");

            var extractionDirectory = System.IO.Path.Combine(_installBase.DownloadsDirectory, Path.GetFileNameWithoutExtension(mod.FileName));
            var archivePath = mod.FilePath;
            var installPath = System.IO.Path.Combine(_installBase.InstallDirectory, _installBase.ModpackHeader.Name, "mods", mod.ModName);

            if (mod.InstallParameters != null)
            {
                var selections = mod.InstallParameters
                                    .GroupBy(e => e.SourceLocation)
                                    .ToDictionary(e => e.Key);

                _archiveContents.ExtractAll(archivePath,
                        e => selections.ContainsKey(e) ? Path.Combine(installPath, selections[e].First().TargetLocation.Substring(1)) : null);

                // Some mods write the same file to two locations and the extractor
                // doesn't support this, so we'll copy the file around a bit.
                foreach (var selection in selections.Where(s => s.Value.Count() > 1))
                {
                    var from = Path.Combine(installPath, selection.Value.First().TargetLocation.Substring(1));
                    foreach (var e in selection.Value.Skip(1))
                    {
                        var to = Path.Combine(installPath, e.TargetLocation.Substring(1));

                        if (!Directory.Exists(Path.GetDirectoryName(to)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(to));
                        }

                        if (to != from)
                        {
                            File.Copy(from, to, true);
                        }
                    }
                }
            }

            // Write the meta.ini
            var metaPath = Path.Combine(installPath, "meta.ini");
            File.WriteAllText(metaPath,
                "[General]\n" +
                $"gameName={mod.TargetGame}\n" +
                $"version={mod.Version}\n" +
                $"installationFile={mod.FilePath.Replace(@"\", @"\\")}\n" +
                $"repository={mod.Repository}\n" +
                $"modId={mod.ModId}\n\n" +
                "[installedFiles]\n" +
                $"1\\modId={mod.ModId}\n" +
                $"1\\fileId={mod.FileId}");

            return;
        }

        private void DebugWrite(string message)
        {
            DebugLogCallback.Invoke(this, message);
            _logger.WriteLine(message);
        }
    }
}
