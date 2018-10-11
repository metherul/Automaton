using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using System.Collections.Generic;
using System.Linq;
using Automaton.Model.Instance.Interfaces;

namespace Automaton.Model.Instance
{
    public class FlagInstance : IFlagInstance
    {
        private readonly IAutomatonInstance _automatonInstance;

        public List<FlagKeyValue> FlagKeyValueList
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

        private List<FlagKeyValue> _flagKeyValueList = new List<FlagKeyValue>();
        private readonly List<string> _applicationFlagKeys = new List<string>()
        {
            "$ModInstallFolders" // Can only add or remove
        };

        public FlagInstance(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        public void AddOrModifyFlag(string flagKey, string flagValue, Types.FlagActionType flagActionType)
        {
            // Detect if the flag is attempting to modify a application flag property
            if (_applicationFlagKeys.Contains(flagKey))
            {
                ModifyApplicationFlag(flagKey, flagValue, flagActionType);

                return;
            }

            var matchingFlagKey = FlagKeyValueList.Where(x => x.Key == flagKey).NullAndAny();

            // A key match was found, update the value
            if (matchingFlagKey)
            {
                // Set the value
                if (flagActionType == Types.FlagActionType.Set)
                {
                    FlagKeyValueList.Find(x => x.Key == flagKey).Value = flagValue;
                }

                // Remove the value
                else if (flagActionType == Types.FlagActionType.Remove)
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
        private void ModifyApplicationFlag(string flagKey, string flagValue, Types.FlagActionType flagActionType)
        {
            // Make sure the flagValue isn't null or empty
            if (string.IsNullOrEmpty(flagValue))
            {
                return;
            }

            if (flagKey == "$ModInstallFolders" && !_automatonInstance.ModpackHeader.ModInstallFolders.Where(x => x == flagValue).NullAndAny())
            {
                if (flagActionType == Types.FlagActionType.Add)
                {
                    _automatonInstance.AddModInstallFolder(flagValue);
                }
                else if (flagActionType == Types.FlagActionType.Remove)
                {
                    _automatonInstance.RemoveModInstallFolder(flagValue);
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