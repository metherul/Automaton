using Automaton.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Automaton.ViewModel
{
    public class OptionalsInstallerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<OptionalInstall> OptionalInstalls { get; set; }

        public string InstallerTitle { get; set; }
        public string ImagePath { get; set; }
        public string DescriptionText { get; set; }

        public OptionalsInstallerViewModel()
        {
            GeneratePackOptionals(PackHandler.ModPack);
        }

        private void GeneratePackOptionals(ModPack modPack)
        {
            var workingOptional = modPack.OptionalInstallation;

            InstallerTitle = workingOptional.Title;
            ImagePath = workingOptional.DefaultImage;
            DescriptionText = workingOptional.DefaultDescription;

            // Lets start building the installer's controls

            var workingGroups = workingOptional.Groups;
            OptionalInstalls = new ObservableCollection<OptionalInstall>();

            foreach (var group in workingGroups)
            {
                var optionalInstall = new OptionalInstall()
                {
                    Header = group.Header,
                    ChildControls = new List<dynamic>()
                };

                // TODO: Set types to some sort of global enum or dictionary instead of hard-coding them in here.

                var stackPanel = new StackPanel();
                foreach (var element in group.Elements)
                {
                    if (element.Type == "checkbox")
                    {
                        var checkbox = new CheckBox()
                        {
                            Content = element.Text
                        };

                        // Checkbox event handler binding. 
                        checkbox.MouseEnter += Element_Hover;
                        checkbox.Checked += Element_Checked;
                        checkbox.Unchecked += Element_UnChecked;

                        // Fuck it, lets just stick the entire element on the command parameter. Can't see how that would go wrong.
                        checkbox.CommandParameter = element;

                        stackPanel.Children.Add(checkbox);
                    }

                    if (element.Type == "radiobutton")
                    {
                        var radioButton = new RadioButton()
                        {
                            Content = element.Text,
                        };

                        // Radiobutton event handler binding
                        radioButton.MouseEnter += Element_Hover;
                        radioButton.Checked += Element_Checked;
                        radioButton.Unchecked += Element_UnChecked;

                        // Here it is again.
                        radioButton.CommandParameter = element;

                        stackPanel.Children.Add(radioButton);
                    }
                }

                optionalInstall.ChildControls.Add(stackPanel);
                OptionalInstalls.Add(optionalInstall);
            }
        }

        private void Element_Checked(dynamic sender, EventArgs e)
        {
            FlagWorker((Element)sender.CommandParameter, "checked");
        }
        private void Element_UnChecked(dynamic sender, EventArgs e)
        {
            FlagWorker((Element)sender.CommandParameter, "unchecked");
        }
        private void Element_Hover(dynamic sender, EventArgs e)
        {
            var controlData = (Element)sender.CommandParameter;

            DescriptionText = controlData.Description;
            ImagePath = controlData.Image;
        }

        private string AddTwoStrings(string one, string two)
        {
            Int32.TryParse(one, out int iOne);
            Int32.TryParse(two, out int iTwo);
            return (iOne + iTwo).ToString();
        }
        private string SubtractTwoStrings(string one, string two)
        {
            Int32.TryParse(one, out int iOne);
            Int32.TryParse(two, out int iTwo);
            return (iOne - iTwo).ToString();
        }

        private void FlagWorker(Element controlData, string eventType)
        {
            var flags = controlData.Flags.Where(x => x.Event == eventType).ToList();

            foreach (var flag in flags)
            {
                Debug.WriteLine($"FLAG NAME: {flag.Name}\nFLAG VALUE: {flag.Value}\nFLAG EVENT: {flag.Event}\nFLAG ACTION: {flag.Action}\n");

                if (FlagHandler.FlagList.Where(x => x.FlagName == flag.Name).Count() == 0)
                {
                    // Add a new flag to the FlagHandler since it does not exist. 
                    FlagHandler.FlagList.Add(new Model.StorageFlag()
                    {
                        FlagName = flag.Name,
                        FlagValue = flag.Value
                    });

                    // To prevent nesting, skip the remainder of the foreach loop
                    continue;
                }
                    
                var tempFlag = FlagHandler.FlagList.Where(x => x.FlagName == flag.Name).First();

                if (flag.Action == null || flag.Action == "set")
                {
                    tempFlag.FlagValue = flag.Value;
                }

                if (flag.Action == "add")
                {
                    if (Regex.IsMatch(tempFlag.FlagValue, @"^\d+$")) // Will result true if it's an int
                    {
                        tempFlag.FlagValue = AddTwoStrings(tempFlag.FlagValue, flag.Value);
                    }

                    else
                    {
                        tempFlag.FlagValue += flag.Value;
                    }
                }

                if (flag.Action == "subtract")
                {
                    if (Regex.IsMatch(tempFlag.FlagValue, @"^\d+$")) // Will result true if it's an int
                    {
                        tempFlag.FlagValue = SubtractTwoStrings(tempFlag.FlagValue, flag.Value);
                    }

                    else
                    {
                        // Janky, but it should work. If it doesn not find a matching value in the string, it shouldn't affect the value at all.
                        tempFlag.FlagValue.Replace(flag.Value, "");
                    }
                }
            }

            Debug.WriteLine("STORED FLAGS:");
            foreach (var flag in FlagHandler.FlagList)
            {
                Debug.WriteLine($"({flag.FlagName}, {flag.FlagValue})");
            }
            Debug.WriteLine("");
        }
    }

    public class OptionalInstall
    {
        public string Header { get; set; }
        public Grid PrimaryGrid { get; set; }
        public List<dynamic> ChildControls { get; set; }
    }

    public class StorageFlag
    {
        public string FlagName { get; set; }
        public string FlagValue { get; set; }
    }
}
