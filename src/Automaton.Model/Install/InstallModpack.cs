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
            foreach (var mod in _installBase.ModpackMods)
            {
                InstallMod(mod);
            }
        }

        private void InstallMod(ExtendedMod mod)
        {
            // Install Mod Organizer


            var extractionDirectory = Path.Combine(_installBase.DownloadsDirectory, Path.GetFileNameWithoutExtension(mod.FileName));
            var archivePath = mod.FilePath;
            var installPath = Path.Combine(_installBase.InstallDirectory, _installBase.ModpackHeader.Name, "mods", mod.ModName);

            // Extract mod archive
            _archiveContents.ExtractToDirectory(archivePath, extractionDirectory);

            var extractedArchiveFiles = Directory.GetFiles(extractionDirectory, "*.*", SearchOption.AllDirectories);

            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

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
                $"modId={mod.ModId}\n\n" +
                "[installedFiles]\n" +
                $"1\\modId={mod.ModId}\n" +
                $"1\\fileId={mod.FileId}");

            Directory.Delete(extractionDirectory, true);
        }
    }
}
