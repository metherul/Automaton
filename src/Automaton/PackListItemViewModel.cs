using System;
using System.Collections.Generic;
using System.Text;

namespace Automaton
{
    public class PackListItemViewModel
    { 
        public string Content { get; set; }
        public PackListItemViewModel(string test)
        {
            Content = test;
        }
    }
}
