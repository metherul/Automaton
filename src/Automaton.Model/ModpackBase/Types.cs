namespace Automaton.Model.ModpackBase
{
    public class Types
    {
        public enum ControlType
        {
            CheckBox,
            RadioButton
        }

        public enum FlagEventType
        {
            Checked,
            UnChecked
        }

        public enum FlagActionType
        {
            Add,
            Remove,
            Subtract,
            Set
        }
    }
}
