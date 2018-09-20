using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using System.Collections.Generic;
using System.Linq;

namespace Automaton.Model.Instance
{
    public class FlagInstance
    {
        private static List<FlagKeyValue> _flagKeyValueList = new List<FlagKeyValue>();

        public static List<FlagKeyValue> FlagKeyValueList
        {
            get => _flagKeyValueList;
            set
            {
                if (_flagKeyValueList != value)
                {
                    _flagKeyValueList = value;
                }
            }
        }

        private static readonly List<string> ApplicationFlagKeys = new List<string>()
        {
            "$ModInstallFolders" // Can only add or remove
        };

        public static void AddOrModifyFlag(string flagKey, string flagValue, FlagActionType flagActionType)
        {
            // Detect if the flag is attempting to modify a application flag property
            if (ApplicationFlagKeys.Contains(flagKey))
            {
                ModifyApplicationFlag(flagKey, flagValue, flagActionType);

                return;
            }

            var matchingFlagKey = FlagKeyValueList.Where(x => x.Key == flagKey).ContainsAny();

            // A key match was found, update the value
            if (matchingFlagKey)
            {
                // Set the value
                if (flagActionType == FlagActionType.Set)
                {
                    FlagKeyValueList.Find(x => x.Key == flagKey).Value = flagValue;
                }

                // Remove the value
                else if (flagActionType == FlagActionType.Remove)
                {
                    FlagKeyValueList.RemoveAll(x => x.Key == flagKey);
                }
            }

            // No matching keys were found
            else
            {
                FlagKeyValueList.Add(new FlagKeyValue()
                {
                    Key = flagKey,
                    Value = flagValue
                });
            }
        }

        /// <summary>
        /// Exposes the ModpackInstance to allow for runtime modpack modifications
        /// </summary>
        /// <param name="flagKey"></param>
        /// <param name="flagValue"></param>
        /// <param name="flagActionType"></param>
        private static void ModifyApplicationFlag(string flagKey, string flagValue, FlagActionType flagActionType)
        {
            // Make sure the flagValue isn't null or empty
            if (string.IsNullOrEmpty(flagValue))
            {
                return;
            }

            if (flagKey == "$ModInstallFolders" && !AutomatonInstance.ModpackHeader.ModInstallFolders.Where(x => x == flagValue).ContainsAny())
            {
                if (flagActionType == FlagActionType.Add)
                {
                    AutomatonInstance.AddModInstallFolder(flagValue);
                }
                else if (flagActionType == FlagActionType.Remove)
                {
                    AutomatonInstance.RemoveModInstallFolder(flagValue);
                }
            }
        }
    }

    public class FlagKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}