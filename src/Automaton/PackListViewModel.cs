using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automaton
{
    public class PackListViewModel
    {
        public BindableCollection<PackListItemViewModel> PackListItems { get; set; }

        public PackListViewModel()
        {

            PackListItems = new BindableCollection<PackListItemViewModel>();

            for (var i = 0; i < 10; i++)
            {
                PackListItems.Add(new PackListItemViewModel("Ultimate Skyrim v " + i));
            }
        }
    }
}
