using Automaton.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class TransitionHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentCardIndex { get; set; }
        public bool IsCardOpen { get; set; }

        public TransitionHandler()
        {
            Messenger.Default.Register<CardIndex>(this, x => CurrentCardIndex = Convert.ToInt32(x));
            Messenger.Default.Register<CardControl>(this, x => IsCardOpen = false);

            CurrentCardIndex = 0;
            IsCardOpen = true;
        }

        public static void CalculateNextCard(CardIndex currentIndex)
        {
            var modPack = PackHandler.ModPack;
            var missingMods = PackHandler.ValidateSourceLocation();

            if (currentIndex == CardIndex.InitialSetup)
            {
                if (PackHandlerHelper.DoOptionalsExist(modPack))
                {
                    Messenger.Default.Send(CardIndex.OptionalSetup);
                }

                else if (missingMods.Count > 0)
                {
                    PackHandler.ValidateSourceLocation();
                    Messenger.Default.Send(CardIndex.ModValidation);
                }

                else
                {
                    Messenger.Default.Send(CardIndex.CompletedSetup);
                }
            }

            else if (currentIndex == CardIndex.OptionalSetup)
            {
                PackHandler.FilterModPack();

                if (missingMods.Count > 0)
                {
                    PackHandler.ValidateSourceLocation();
                    Messenger.Default.Send(CardIndex.ModValidation);
                }

                else
                {
                    Messenger.Default.Send(CardIndex.CompletedSetup);
                }
            }

            else if (currentIndex == CardIndex.ModValidation)
            {
                Messenger.Default.Send(CardIndex.CompletedSetup);
            }
        }
    }

    public enum CardIndex
    {
        InitialSetup = 0,
        OptionalSetup = 1,
        ModValidation = 2,
        CompletedSetup = 3
    }

    public enum CardControl
    {
        CloseCard
    }
}
