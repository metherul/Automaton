using Automaton.Model.Interfaces;
using System;

namespace Automaton.Model.Install.Interfaces
{
    public interface IInstallModpack : IService
    {
        EventHandler<string> DebugLogCallback { get; set; }

        void Install();
    }
}