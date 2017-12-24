using ExtraTools;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Automaton.Model
{
    class PackHandler
    {
        private static ModPack modPack;
        public static ModPack ModPack
        {
            get { return modPack; }
            set
            {
                modPack = value;

                if (modPack != null)
                {
                    Messenger.Default.Send(modPack, MessengerToken.ModPack);
                }
            }

        }

        private static ModPack finalModPack;
        public static ModPack FinalModPack
        {
            get { return finalModPack; }
            set
            {
                finalModPack = value;

                if (finalModPack != null)
                {
                    Messenger.Default.Send(finalModPack, MessengerToken.FinalModPack);
                }
            }
        }

        public static string PackLocation;
        public static string SourceLocation;
        public static string InstallationLocation;

        private static string ModPackContents;

        public static ModPack ReadPack(string packFileLocation)
        {
            // Decompress and place into the temp directory
            var metaLocation = AppDomain.CurrentDomain.BaseDirectory;
            var tempDirectory = Path.Combine(metaLocation, "temp");

            using (var sevenZipHandler = new SevenZipHandler(packFileLocation))
            {
                var modPackName = Path.GetFileNameWithoutExtension(packFileLocation);
                var modPackExtractedPath = Path.Combine(tempDirectory, modPackName);
                var contentDirectory = Path.Combine(modPackExtractedPath, "content");

                packFileLocation = Path.Combine(modPackExtractedPath, "modpack.json");

                if (File.Exists(packFileLocation))
                {
                    ModPackContents = File.ReadAllText(packFileLocation);
                }

                else
                {
                    return null;
                }
            }

            try
            {
                var modPack = JsonConvert.DeserializeObject<ModPack>(ModPackContents);

                ModPack = modPack;

                return modPack;
            }

            catch (Exception e)
            {
                DialogBox.Show("ERROR", e.Message, "COPY", "ABORT");
                if (DialogBox.Result == DialogBox.ResultEnum.LeftButtonClicked)
                {
                    Clipboard.SetText(e.Message);
                }

                return null;
            }
        }

        public static void WritePack(ModPack modPack, string target)
        {
            File.WriteAllText(target, JsonConvert.SerializeObject(modPack, Formatting.Indented));
        }

        public static ModPack GenerateFinalModPack()
        {
            // Will compare the FilterList and ModPack for all required mods -- remove anything which doesn't match conditional parameters.
            var workingModPack = ModPack;
            var mods = workingModPack.Mods;
            var modsToRemove = new List<Mod>();

            foreach (var mod in mods)
            {
                var conditionals = new List<Conditional>();
                var doesCollectionHaveElements= mod.Installations.Where(x => x.Conditionals != null).Any();

                if (doesCollectionHaveElements)
                {
                    conditionals = mod.Installations.SelectMany(x => x.Conditionals).ToList();

                    foreach (var conditional in conditionals)
                    {
                        if (FlagHandler.FlagList.Where(x => x.FlagName == conditional.Name && x.FlagValue != conditional.Value).Count() > 0)
                        {
                            // Matching names, but different values were found. 
                            modsToRemove.Add(mod);
                        }

                        else if (FlagHandler.FlagList.Where(x => x.FlagName == conditional.Name).Count() == 0)
                        {
                            // No matching names, the conditional is missing it's flag. 
                            modsToRemove.Add(mod);
                        }
                    }
                }
            }

            workingModPack.Mods = workingModPack.Mods.Where(x => !modsToRemove.Contains(x)).ToList();

            FinalModPack = workingModPack;

            return workingModPack;
        }

        public static List<Mod> ValidateSourceLocation(string sourceLocation)
        {
            var sourceFiles = Directory.GetFiles(sourceLocation);
            var sourceFileSizes = sourceFiles.Select(x => new FileInfo(x));
            var workingModPack = FinalModPack;
            var missingMods = new List<Mod>();

            foreach (var mod in workingModPack.Mods)
            {
                // Gets count of source files which match the file size of the filter. 
                var filteredFilesBySize = sourceFileSizes.Where(x => x.Length.ToString() == mod.FileSize);

                // When there are no source file size matches
                if (filteredFilesBySize.Count() == 0)
                {
                    missingMods.Add(mod);
                }
            }

            return missingMods;
        }

        public static void InstallModPack(ModPack modPack, string sourceLocation, string installationLocation)
        {
            var sourceFiles = GetSourceFiles(modPack, sourceLocation);
            var mods = modPack.Mods;

            // There were no sourceFiles found, return. 
            // Shouldn't be used when proper mod validation is enabled.
            if (sourceFiles.Count() == 0)
            {
                return;
            }

            foreach (var mod in mods)
            {
                var workingModFile = sourceFiles.Where(x => x.Length.ToString() == mod.FileSize).First();
                var installations = mod.Installations;

                using (var extractHandler = new SevenZipHandler(workingModFile.FullName))
                {
                    foreach (var installation in installations)
                    {
                        var sourcePath = installation.Source;
                        var targetPath = installation.Target;
                        var outputPath = Path.Combine(installationLocation, targetPath);

                        extractHandler.Install(sourcePath, installationLocation, targetPath);
                    }
                }
            }
        }

        public static bool DoesOptionalExist(ModPack modPack)
        {
            var modPackOptionals = modPack.OptionalInstallation;

            if (modPackOptionals == null || modPackOptionals.Title == null || modPackOptionals.Groups == null)
            {
                return false;
            }

            if (modPackOptionals.Groups.Count == 0)
            {
                return false;
            }

            return true;
        }

        private static List<FileInfo> GetSourceFiles(ModPack modPack, string sourceLocation)
        {
            var sourceFiles = Directory.GetFiles(sourceLocation);
            var sourceFileInfos = sourceFiles.Select(x => new FileInfo(x)).ToList();
            var modPackFiles = modPack.Mods;
            var matchingSourceFiles = new List<FileInfo>();

            foreach (var mod in modPackFiles)
            {
                var matchingMod = sourceFileInfos.Where(x => x.Length.ToString() == mod.FileSize);

                if (matchingMod.Count() > 0)
                {
                    matchingSourceFiles.Add(matchingMod.First());
                }
            }

            // Sort the matching files into the order defined by the mod pack

            return matchingSourceFiles;
        }
    }
}
