using Automaton.Model.Interfaces;

namespace Automaton.Model
{
    public class LifetimeData : ILifetimeData
    {
        public string RequestHeader { get; set; }
        public string InstallPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string DownloadPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ApiKey { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
