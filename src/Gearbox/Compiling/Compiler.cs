using Gearbox.Indexing;
using System.Threading.Tasks;

namespace Gearbox.Compiling
{
    public class Compiler
    {
        private readonly IndexBase _indexBase;

        public Compiler(IndexBase indexBase)
        {
            _indexBase = indexBase;
        }
        public async Task<CompilerWriter> LoadSources()
        {
            var compilerWriter = new CompilerWriter();
            return await compilerWriter.LoadSources(_indexBase);
        }
    }
}