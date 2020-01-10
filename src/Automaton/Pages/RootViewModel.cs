using Automaton.UserControls;
using Gearbox.Indexing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using Gearbox;
using Gearbox.Compiling;
using Gearbox.Modpacks;

namespace Automaton.Pages
{
    public class RootViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskbarViewModel Taskbar { get; private set; }
        public BrowseModpackViewModel BrowseModpack { get; private set; }

        public ObservableCollection<string> DebugOut { get; set; } = new ObservableCollection<string>();

        public RootViewModel()
        {
            Taskbar = new TaskbarViewModel();
            BrowseModpack = new BrowseModpackViewModel();

            Task.Run(() => Run());
        }

        public async Task Run()
        {
            // var index = new IndexBase(@"E:\Mod Organizer\Ultimate Skyrim 4.0.5 (Full)\ModOrganizer.exe");
            // var compiler = new Compiler(index);
            // var writer = await compiler.LoadSources();
            //
            // await writer.CompileModpack(new CompilerOptions()
            // {
            //     ModpackName = "TestModpack",
            //     Author = "Metherul",
            //     Version = "0.0.1"
            // });

            var pack = await PackFactory.FromFile(
                @"E:\Mod Organizer\Ultimate Skyrim 4.0.5 (Full)\automaton\build\TestModpack.oms");

            foreach (var source in pack.Sources)
            {
                var test = source.Repository;
            }
        }
    }
}
