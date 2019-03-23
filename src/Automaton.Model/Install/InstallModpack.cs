using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Install.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automaton.Model.Install
{
    public class InstallModpack : IInstallModpack
    {
        private readonly IInstallBase _installBase;
        private readonly IArchiveContents _archiveContents;

        private int _maxConcurrency = 3;

        public EventHandler<string> DebugLogCallback { get; set; }

        public InstallModpack(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _archiveContents = components.Resolve<IArchiveContents>();
        }

        public void Install()
        {
            // Install Mod Organizer
            var installPath = Path.Combine(_installBase.InstallDirectory, _installBase.ModpackHeader.Name);

            if (_installBase.ModpackHeader.InstallModOrganizer)
            {
                DebugWrite("[INSTALL] Installing Mod Organizer...");
                var modOrganizerObject = _installBase.ModpackMods.First(x => x.IsModOrganizer);

                _archiveContents.ExtractToDirectory(modOrganizerObject.FilePath, installPath);
                _installBase.ModpackMods.Remove(modOrganizerObject);
            }

            DebugWrite("_CLEAR");


            foreach (var mod in _installBase.ModpackMods)
            {
                InstallMod(mod);

                DebugWrite("_CLEAR");
            }

            // Write the needed profile information
            var profilePath = Path.Combine(installPath, "profiles", _installBase.ModpackHeader.Name);

            if (Directory.Exists(profilePath))
            {
                Directory.Delete(profilePath, true);
            }

            Directory.CreateDirectory(profilePath);

            File.WriteAllText(Path.Combine(profilePath, "plugins.txt"), _installBase.PluginsTxt);
            File.WriteAllText(Path.Combine(profilePath, "loadorder.txt"), _installBase.LoadorderTxt);
            File.WriteAllText(Path.Combine(profilePath, "modlist.txt"), _installBase.ModlistTxt);
            File.WriteAllText(Path.Combine(profilePath, "archives.txt"), _installBase.ModlistTxt);
            File.WriteAllText(Path.Combine(profilePath, "lockedorder.txt"), _installBase.ModlistTxt);

            DebugWrite("[DONE] Operation completed.");
            DebugWrite("_END");
        }

        private void InstallMod(ExtendedMod mod)
        {
            DebugWrite($"[INSTALL] {mod.ModName}");
            DebugWrite($"[INSTALL] {mod.FileName}");

            var extractionDirectory = Path.Combine(_installBase.DownloadsDirectory, Path.GetFileNameWithoutExtension(mod.FileName));
            var archivePath = mod.FilePath;
            var installPath = Path.Combine(_installBase.InstallDirectory, _installBase.ModpackHeader.Name, "mods", mod.ModName);

            // Extract mod archive
            DebugWrite("[!] Extracting files...");
            _archiveContents.ExtractToDirectory(archivePath, extractionDirectory);

            DebugWrite("[!] Enumerating files...");
            var extractedArchiveFiles = Directory.GetFiles(extractionDirectory, "*.*", SearchOption.AllDirectories);

            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

            DebugWrite("[!] Installing mod...");
            foreach (var installParameter in mod.InstallParameters)
            {
                var matchingSourceFile = extractedArchiveFiles.Where(x => x.Replace(extractionDirectory, "") == installParameter.SourceLocation);
                var installFilePath = Path.Combine(installPath, installParameter.TargetLocation.Remove(0, 1));

                if (matchingSourceFile == null || !matchingSourceFile.Any())
                {
                    continue;
                }

                if (File.Exists(installFilePath))
                {
                    File.Delete(installFilePath);
                }

                if (!Directory.Exists(Path.GetDirectoryName(installFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(installFilePath));
                }

                File.Copy(matchingSourceFile.First(), installFilePath);
            }

            // Write the meta.ini
            var metaPath = Path.Combine(installPath, "meta.ini");
            File.WriteAllText(metaPath,
                "[General]\n" +
                $"gameName={mod.TargetGame}\n" +
                $"version={mod.Version}\n" +
                $"installationFile={mod.FilePath}\n" +
                $"repository={mod.Repository}\n" + 
                $"modId={mod.ModId}\n\n" +
                "[installedFiles]\n" +
                $"1\\modId={mod.ModId}\n" +
                $"1\\fileId={mod.FileId}");

            DebugWrite("[!] Removing extracted files...");
            Directory.Delete(extractionDirectory, true);
        }

        private void DebugWrite(string message)
        {
            DebugLogCallback.Invoke(this, message);
        }
    }
}
