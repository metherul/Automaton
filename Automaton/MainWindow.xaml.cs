using Automaton.Model;
using System.Collections.Generic;
using System.Windows;

namespace Automaton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var debug = false;

            InitializeComponent();

            if (debug)
            {
                var modPack = new ModPack()
                {
                    PackName = "Test Pack",
                    PackAuthor = "metherul",
                    PackVersion = "v0",
                    SourceUrl = "test.com",
                    OptionalInstallation = new OptionalInstallation()
                    {
                        Title = "Test Header",
                        DefaultImage = "defaultImage.jpg",
                        DefaultDescription = "Welcome to the very first test optional installer!",
                        Groups = new List<Group>()
                        {
                            new Group()
                            {
                                Header = "Test Header",
                                Elements = new List<Element>()
                                {
                                    new Element()
                                    {
                                        Type = "checkbox",
                                        Text = "Test Checkbox",
                                        Description = "This is a test description",
                                        Flags = new List<Flag>()
                                        {
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            },
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            }
                                        }
                                    },
                                    new Element()
                                    {
                                        Type = "checkbox",
                                        Text = "Test Checkbox",
                                        Description = "This is a test description",
                                        Flags = new List<Flag>()
                                        {
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            },
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            }
                                        }
                                    }
                                }
                            },
                            new Group()
                            {
                                Header = "Test Header",
                                Elements = new List<Element>()
                                {
                                    new Element()
                                    {
                                        Type = "checkbox",
                                        Text = "Test Checkbox",
                                        Description = "This is a test description",
                                        Flags = new List<Flag>()
                                        {
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            },
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            }
                                        }
                                    },
                                    new Element()
                                    {
                                        Type = "checkbox",
                                        Text = "Test Checkbox",
                                        Description = "This is a test description",
                                        Flags = new List<Flag>()
                                        {
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            },
                                            new Flag()
                                            {
                                                Name = "someFlag",
                                                Event = "focus",
                                                Value = "1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },

                    Mods = new List<Mod>()
                    {
                        new Mod()
                        {
                            ModName = "Test Mod Name",
                            FileName = "fileName.7z",
                            FileSize = "231321312123",
                            CheckSum = "DAWIODWAIO78DW8A78W7D8989721389712",
                            LoadOrder = 1,
                            Installations = new List<Installation>()
                            {
                                new Installation()
                                {
                                    Source = "sourceFile.bmp",
                                    Target = "targetFile.bmp",
                                    Conditionals = new List<Conditional>()
                                    {
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        },
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        }
                                    },
                                    Ignores = new List<string>()
                                    {
                                        "someFile",
                                        "someOtherFile"
                                    }
                                },
                                new Installation()
                                {
                                    Source = "sourceFile.bmp",
                                    Target = "targetFile.bmp",
                                    Conditionals = new List<Conditional>()
                                    {
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        },
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        }
                                    },
                                    Ignores = new List<string>()
                                    {
                                        "someFile",
                                        "someOtherFile"
                                    }
                                }
                            }

                        },
                        new Mod()
                        {
                            ModName = "Test Mod Name",
                            FileName = "fileName.7z",
                            FileSize = "231321312123",
                            CheckSum = "DAWIODWAIO78DW8A78W7D8989721389712",
                            LoadOrder = 1,
                            Installations = new List<Installation>()
                            {
                                new Installation()
                                {
                                    Source = "sourceFile.bmp",
                                    Target = "targetFile.bmp",
                                    Conditionals = new List<Conditional>()
                                    {
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        },
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        }
                                    },
                                    Ignores = new List<string>()
                                    {
                                        "someFile",
                                        "someOtherFile"
                                    }
                                },
                                new Installation()
                                {
                                    Source = "sourceFile.bmp",
                                    Target = "targetFile.bmp",
                                    Conditionals = new List<Conditional>()
                                    {
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        },
                                        new Conditional()
                                        {
                                            Name = "someFlag",
                                            Value = "1"
                                        }
                                    },
                                    Ignores = new List<string>()
                                    {
                                        "someFile",
                                        "someOtherFile"
                                    }
                                }
                            }

                        }
                    }
                };

                //PackHandler.WritePack(modPack, @"C:\Programming\Automaton\Automaton\bin\Debug\someOtherTest.json");
            }
        }
    }
}
