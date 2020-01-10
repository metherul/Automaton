namespace Gearbox.Modpacks
{
    public interface IHeader
    {
        string Name { get; }
        string Author { get; }
        string Version { get; }
    }
}