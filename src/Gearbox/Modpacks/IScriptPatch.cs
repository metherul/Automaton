using System;
using System.Collections.Generic;
using System.Text;

namespace Gearbox.Modpacks
{
    public interface IScriptPatch 
    {
        string Name { get; set; }
        string Authors { get; set; }
        string Repo { get; set; } 
    }
}
