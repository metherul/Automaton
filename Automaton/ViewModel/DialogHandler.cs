using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class DialogHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentCardIndex { get; set; }
        public bool IsCardOpen { get; set; }

        public DialogHandler()
        {
            Messenger.Default.Register<CardIndex>(this, SetCard);
            Messenger.Default.Register<CardControl>(this, IsCardVisible);

            CurrentCardIndex = 0;
            IsCardOpen = true;
        }

        public void SetCard(CardIndex cardIndex)
        {
            CurrentCardIndex = Convert.ToInt32(cardIndex);
        }

        public void IsCardVisible(CardControl value)
        {
            IsCardOpen = false;
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
        IsCardOpen
    }
}
