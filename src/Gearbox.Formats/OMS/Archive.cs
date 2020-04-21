namespace Gearbox.Formats.OMS
{
    public class Archive
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
        public string FilesystemHash { get; set; }
        public long Length { get; set; }
        public object Repository { get; set; }
    }
}