using Alphaleonis.Win32.Filesystem;
using Automaton.Common;
using SevenZipExtractor;
using System.Linq;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public partial class ExtendedArchive
    {
        public void Install()
        {
            var installationDirectory = Path.Combine(_lifetimeData.InstallPath, "mods", _parentMod.Name);
            //var filePairings = _parentMod.InstallPlans.SelectMany(x => x.FilePairings);

            var plan = _parentMod.InstallPlans.Where(p => p.SourceArchive.SHA256 == SHA256).First();

            // Verify to ensure that the SourceArchive path exists
            if (!File.Exists(ArchivePath))
            {
                if (!File.Exists(_boundArchive.ArchivePath))
                {
                    return;
                }

                ArchivePath = _boundArchive.ArchivePath;
            }

            // Verify to ensure that the mod's installation directory exists
            if (!Directory.Exists(installationDirectory))
            {
                Directory.CreateDirectory(installationDirectory);
            }

            // Get a dictionary of all the files we need to copy indexed by their name in the archive
            var extract_files = plan.FilePairings.GroupBy(p => p.From).ToDictionary(p => p.Key);


            // Let's pre-create all the directories in the mod folder so we don't have to check
            // for missing folders during the install.
            var directories = (from entry in extract_files
                               from to in entry.Value
                               let full_path = Path.Combine(installationDirectory, to.To)
                               select Path.GetDirectoryName(full_path)).Distinct();

            foreach (var dir in directories)
                Directory.CreateDirectory(dir);


            using (var file = new ArchiveFile(ArchivePath))
            {
                file.Extract(entry => {
                    if (extract_files.ContainsKey(entry.FileName))
                    {
                        // We may need to copy the same file to multiple locations so extract to the first one, 
                        // we'll copy this file around later.
                        var to = extract_files[entry.FileName].First();
                        var path = Path.Combine(installationDirectory, to.To);
                        return File.OpenWrite(Path.Combine(installationDirectory, path));
                    }

                    return null;
                });
            }

            // Now that we've installed all the files, copy around any files that exist in more than one location
            // in the mod.
            foreach (var copy_group in extract_files.Select(e => e.Value).Where(es => es.Count() > 1))
            {
                var from = copy_group.First();
                foreach (var to in copy_group.Skip(1))
                {
                    File.Copy(Path.Combine(installationDirectory, from.To),
                              Path.Combine(installationDirectory, to.To));
                }
            }

            foreach (var to_patch in plan.FilePairings.Where(p => p.patch_id != null))
            {
                using (var patch_stream = new System.IO.MemoryStream())
                {
                    // Read in the patch data
                    Patches[to_patch.patch_id].Extract(patch_stream);
                    patch_stream.Seek(0, System.IO.SeekOrigin.Begin);

                    System.IO.MemoryStream old_data = new System.IO.MemoryStream();
                    var to_file = Path.Combine(installationDirectory, to_patch.To);
                    // Read in the unpatched file
                    using (var unpatched = File.OpenRead(to_file))
                    {
                        unpatched.CopyTo(old_data);
                        old_data.Seek(0, System.IO.SeekOrigin.Begin);
                    }

                    // Patch it
                    using (var out_stream = File.OpenWrite(to_file))
                    {
                        var ps = new PatchingStream(old_data, patch_stream, out_stream, patch_stream.Length);
                        ps.Patch();
                    }
                }

            }

            // And we're done!


        }

        public async Task InstallAsync()
        {
            await Task.Run(Install);
        }
    }
}
