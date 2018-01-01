using Automaton.Handles;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    class CompletedDialogViewModel
    {
        public RelayCommand NextCardCommand { get; set; }

        public CompletedDialogViewModel()
        {
            NextCardCommand = new RelayCommand(NextCard);
        }

        public void NextCard()
        {
            TransitionHandler.CalculateNextCard(CardIndex.CompletedSetup);
        }
    }
}
