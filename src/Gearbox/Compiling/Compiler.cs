using Gearbox.Indexing;
using System.Threading.Tasks;

namespace Gearbox.Compiling
{
    public class Compiler
    {
        private readonly Index _indexBase;

        public Compiler(Index indexBase)
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