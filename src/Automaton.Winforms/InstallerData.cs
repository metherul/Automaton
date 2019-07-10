using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automaton.Common.Model;

namespace Automaton.Winforms
{
    public class InstallerData
    {
        public MasterDefinition MasterDefinition { get; internal set; }
        public List<Mod> Mods { get; internal set; }
    }
}
