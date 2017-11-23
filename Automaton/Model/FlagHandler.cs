using System.Collections.Generic;

namespace Automaton.Model
{
    public static class FlagHandler
    {
        private static List<StorageFlag> flagList;
        public static List<StorageFlag> FlagList
        {
            get
            {
                if (flagList == null)
                {
                    flagList = new List<StorageFlag>();
                    return flagList;
                }

                return flagList;
            }

            set
            {
                flagList = value;
            }
        }
    }

    public class StorageFlag
    {
        public string FlagName { get; set; }
        public string FlagValue { get; set; }
    }
}
