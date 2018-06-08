namespace Automaton.Controllers
{
    public class ViewIndexController
    {
        public delegate void ViewIndexChanged(int index);
        public static event ViewIndexChanged ViewIndexChangedEvent;

        private static int _currentViewIndex = 0;
        public static int CurrentViewIndex
        {
            get => _currentViewIndex;
            set
            {
                if (value != _currentViewIndex)
                {
                    _currentViewIndex = value;

                    ViewIndexChangedEvent(_currentViewIndex);
                }
            }
        }

        public static void IncrementCurrentViewIndex()
        {
            CurrentViewIndex++;
        }
    }
}
