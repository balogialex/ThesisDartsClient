using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MobileDartsApp.ViewModels
{
    public class DoubleTriesPageViewModel : ObservableObject
    {
        #region fields
        private int selectedDoubleTry;

        private int currentScore = 0;
        private int inputScore = 0;
        private int possibleScore = 0;

        private bool doubleTry0IsPossible;
        private bool doubleTry1IsPossible;
        private bool doubleTry2IsPossible;
        private bool doubleTry3IsPossible;
        #endregion

        #region properties
        public bool DoubleTry0IsPossible
        {
            get => doubleTry0IsPossible;
            set => SetProperty(ref doubleTry0IsPossible, value);
        }
        public bool DoubleTry1IsPossible {
            get => doubleTry1IsPossible;
            set=> SetProperty(ref doubleTry1IsPossible, value);
        }
        public bool DoubleTry2IsPossible
        {
            get => doubleTry2IsPossible;
            set => SetProperty(ref doubleTry2IsPossible, value);
        }
        public bool DoubleTry3IsPossible
        {
            get => doubleTry3IsPossible;
            set => SetProperty(ref doubleTry3IsPossible, value);
        }
        #endregion

        #region Commands
        public IRelayCommand<object> SelectDoubleCommand { get; }
        #endregion

        public DoubleTriesPageViewModel()
        {
            SelectDoubleCommand = new RelayCommand<object>(selectDoubleExecute); 
        }

        private async void selectDoubleExecute(object? obj)
        {
            if (obj != null)
                selectedDoubleTry = Convert.ToInt16(obj.ToString());

            await MopupService.Instance.PopAllAsync();
        }
        public void SetPossibleDoubleTries(int currentScore, int inputScore)
        {
            this.currentScore = currentScore;
            this.inputScore = inputScore;
            possibleScore = currentScore - inputScore;

            if (possibleScore == 0)
                DoubleTry0IsPossible = false;
            else
                DoubleTry0IsPossible = true;

            if (currentScore > 100 &&
                (currentScore != 110 || currentScore != 107 || currentScore != 104 || currentScore == 101))

            {
                DoubleTry1IsPossible = true;
                DoubleTry2IsPossible = false;
                DoubleTry3IsPossible = false;
            }
            else if (currentScore == 110 || currentScore == 107 || currentScore == 104 || currentScore == 101 ||
                (currentScore <= 100 && currentScore > 50) ||
                (currentScore < 50 && currentScore % 2 == 1))
            {
                DoubleTry1IsPossible = true;
                DoubleTry2IsPossible = true;
                DoubleTry3IsPossible = false;
            }
            else
            {
                DoubleTry1IsPossible = true;
                DoubleTry2IsPossible = true;
                DoubleTry3IsPossible = true;
            }
        }

        public int GetDoubleTries()
        {
            return selectedDoubleTry;
        }
    }
}
