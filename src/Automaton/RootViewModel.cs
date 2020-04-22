using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Gearbox.SDK;
using Gearbox.Shared.FsExtensions;
using Gearbox.Shared.ModOrganizer;

namespace Automaton
{
    public class RootViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PackListViewModel PackList { get; set; }

        public RootViewModel()
        {
            PackList = new PackListViewModel();
            Task.Run(() => Test());
        }

        public async void Test()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var managerExe = @"E:\Mod Organizer\Ultimate Skyrim 4.0.5 (Full)\ModOrganizer.exe";

            if (!Indexer.IndexExists(managerExe))
            {
                await Indexer.CreateIndex(managerExe);
            }

            var indexDir = Indexer.GetIndexDir(managerExe);
            var indexWriter = await Indexer.OpenWrite(indexDir);
            // var indexReader = await Indexer.OpenRead(indexDir);

            // var manager = ModOrganizer.OpenExecutable(managerExe);
            // var compiler = PackCompiler.Bootstrap(await Indexer.OpenRead(indexDir), manager);
            // await compiler.Build(new CompilerOptions()
            // {
            //     Profile = manager.GetProfile("Ultimate Skyrim 4.0.5 (Full)"),
            //     Author = "metherul",
            //     PackName = "test pack"
            // });

            var modOrganizerReader = ModOrganizer.OpenExecutable(managerExe);
            var archives = await modOrganizerReader.FindSourceArchives(new ArchiveSearchOption()
            {
                SearchUserDirectores = true
            });

            var counter = 1;
            foreach (var archive in archives)
            {
                Debug.WriteLine($"[{counter}/{archives.Count}] {new FileInfo(archive).Name}");

                var archiveEntry = await ArchiveEntry.CreateFastAsync(archive);
                indexWriter.Push(archiveEntry);

                counter++;
            }

            await indexWriter.Flush();

            var mods = await modOrganizerReader.GetModDirs();

            counter = 1;
            foreach (var mod in mods)
            {
                Debug.WriteLine($"[{counter}/{mods.Length}] {new DirectoryInfo(mod).Name}");

                var modEntry = await ModEntry.CreateAsync(mod);
                indexWriter.Push(modEntry);

                counter++;
            }

            await indexWriter.Flush();
            stopwatch.Stop();
        }
    }
}
