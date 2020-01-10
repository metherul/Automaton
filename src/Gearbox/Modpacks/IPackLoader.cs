using System.Threading.Tasks;

namespace Gearbox.Modpacks
{
    public interface IPackLoader
    {
        Task<IPack> Load(string packPath);
    }

}