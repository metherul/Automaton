using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install
{
    public class Validate : IValidate
    {
        private readonly IInstallBase _installBase;
        private readonly ILogger _logger;

        private List<FileInfo> _downloadDirectoryEntries;

        public Validate(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _logger = components.Resolve<ILogger>();
        }

        public string BatchFileMatch(ExtendedMod mod, List<string> directoryFiles)
        {
            var matchingFileSize = directoryFiles.Where(x => File.GetSize(x).ToString() == mod.FileSize).ToList();

            if (!matchingFileSize.Any())
            {
                return string.Empty;
            }

            if (matchingFileSize.Count == 1)
            {
                return matchingFileSize.First();
            }

            if (matchingFileSize.Count > 1)
            {
                foreach (var matchingFile in matchingFileSize)
                {
                    if (GetFileMd5(matchingFile) == mod.Md5)
                    {
                        return matchingFile;
                    }
                }
            }

            return string.Empty;
        }

        public bool IsArchiveMatch(ExtendedMod mod, string archivePath)
        {
            return GetFileMd5(archivePath) == mod.Md5.ToLowerInvariant();
        }

        private string GetFileMd5(string archivePath)
        {
            using (var stream = new System.IO.BufferedStream(File.OpenRead(archivePath), 1200000))
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}
