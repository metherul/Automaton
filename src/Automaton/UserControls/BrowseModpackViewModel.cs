using System.Collections.ObjectModel;

namespace Automaton.UserControls
{
    public class BrowseModpackViewModel
    {
        public ObservableCollection<ModpackViewModel> Modpacks { get; private set; }

        public BrowseModpackViewModel()
        {
            Modpacks = new ObservableCollection<ModpackViewModel>();

            for (var i = 0; i < 10; i++)
            {
                Modpacks.Add(new ModpackViewModel($"Hello there: {i}"));
            }
        }
    }
}
