using System.Threading.Tasks;

namespace Gearbox.Modpacks
{
    public interface ICompilable
    {
        Task MakeFromIndex();
    }
}