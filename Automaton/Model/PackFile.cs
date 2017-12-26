using System.Collections.Generic;

namespace Automaton.Model
{
    public class ModPack
    {
        public string PackName { get; set; }
        public string PackAuthor { get; set; }
        public string PackVersion { get; set; }
        public string SourceUrl { get; set; }
        public OptionalInstallation OptionalInstallation { get; set; }
        public List<Mod> Mods { get; set; }
    }

    public class OptionalInstallation
    {
        public string Title { get; set; }
        public string DefaultImage { get; set; }
        public string DefaultDescription { get; set; }
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        public string Header { get; set; }
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public List<Flag> Flags { get; set; }
    }

    public class Flag
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Event { get; set; }
        public string Action { get; set; }
    }

    public class Mod
    {
        public string ModName { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModLink { get; set; }
        public string CheckSum { get; set; }
        public int? LoadOrder { get; set; }
        public List<Installation> Installations { get; set; }
    }

    public class Installation
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public List<Conditional> Conditionals { get; set; }
        public List<string> Ignores { get; set; }
    }

    public class Conditional
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

