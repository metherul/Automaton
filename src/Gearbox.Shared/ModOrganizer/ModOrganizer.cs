namespace Gearbox.Shared.ModOrganizer
{
    public class ModOrganizer
    {
        public static ManagerReader OpenExecutable(string exePath)
        {
            return new ManagerReader(exePath);
        }
    }
}
