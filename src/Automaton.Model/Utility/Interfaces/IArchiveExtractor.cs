using Automaton.Model.Interfaces;

namespace Automaton.Model.Utility.Interfaces
{
    public interface IArchiveExtractor : IModel
    {
        void Dispose();
        bool ExtractArchive(string extractionPath);
        bool ExtractModpack();
        void TargetArchive(string archivePath);
    }
}