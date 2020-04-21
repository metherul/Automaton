using Gearbox.SDK.Indexers;
using Gearbox.Shared.ModOrganizer;

namespace Gearbox.SDK
{
    public static class PackCompiler
    {
        public static OmsCompiler Bootstrap(IndexReader indexReader, ManagerReader managerReader)
        {
            return new OmsCompiler(indexReader, managerReader);
        }
    }
}
