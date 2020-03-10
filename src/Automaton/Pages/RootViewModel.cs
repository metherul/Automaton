using Automaton.UserControls;
using System.ComponentModel;
using System.Threading.Tasks;
using Gearbox.Modpacks;

namespace Automaton.Pages
{
    public class RootViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskbarViewModel Taskbar { get; private set; }
        public BrowseModpackViewModel BrowseModpack { get; private set; }

        public string DebugOut { get; set; }

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

            const string archive = @"E:\Mod Archive";
            var pack = await PackFactory.FromFile(
                @"E:\Mod Organizer\Ultimate Skyrim 4.0.5 (Full)\automaton\build\TestModpack.oms");

            var manager = new Gearbox.Managers.ModOrganizer.Manager();
            await manager.InstallManager("test");

            foreach (var source in pack.Sources)
            {
                var match = await source.FindMatchInDir(archive);
                match.IfSome((f) =>
                {
                    source.Register(f);
                });
            }

            foreach (var mod in pack.Mods)
            {
                DebugOut = $"Installing: {mod.Name}";
                await manager.InstallMod(mod);
            }
        }
    }
}
