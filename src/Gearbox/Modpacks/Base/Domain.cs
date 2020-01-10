namespace Gearbox.Modpacks.Base
{
    public enum Domain
    {
        Skyrim,
        SkyrimSpecialEdition,
        Fallout4,
        FalloutNV,
        Fallout3,
        Oblivion,
        Morrowind
    }

    public static class DomainExtensions
    {
        public static string ToNexusString(this Domain domain)
        {
            return domain.ToString().ToLower();
        }
    }
}