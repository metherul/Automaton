using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Modpacks
{
    public interface IJsonSource
    {
        Task FromJson(Stream stream);
    }
}