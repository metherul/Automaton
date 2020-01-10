namespace Gearbox.Modpacks
{
    public interface ITheme
    {
        public string ThemeColor { get; }
        public string Background { get; }
        public string TextForeground { get; }
        public string ButtonForeground { get; }
        public string ButtonBackground { get; }
    }
}