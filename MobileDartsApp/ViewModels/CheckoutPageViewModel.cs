using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MobileDartsApp.ViewModels
{     
    public class CheckoutPageViewModel : ObservableObject
    {
        #region fields
        private int selectedTotalThrows;

        private int currentScore = 0;
        private int inputScore = 0;
        private int possibleScore = 0;

        private bool throwCount1IsPossible;
        private bool throwCount2IsPossible;
        private bool throwCount3IsPossible;
        #endregion

        #region properties
        public bool ThrowCount1IsPossible
        {
            get => throwCount1IsPossible;
            set => SetProperty(ref throwCount1IsPossible, value);
        }
        public bool ThrowCount2IsPossible
        {
            get => throwCount2IsPossible;
            set => SetProperty(ref throwCount2IsPossible, value);
        }
        public bool ThrowCount3IsPossible
        {
            get => throwCount3IsPossible;
            set => SetProperty(ref throwCount3IsPossible, value);
        }
        #endregion

        #region Commands
        public IRelayCommand<object> SelectTotalThrowsCommand { get; }
        #endregion
        public CheckoutPageViewModel()
        {
            SelectTotalThrowsCommand = new RelayCommand<object>(SelectTotalThrowsExecute);
        }
        private async void SelectTotalThrowsExecute(object? obj)
        {
            if (obj != null)
                selectedTotalThrows = Convert.ToInt16(obj.ToString());

            await MopupService.Instance.PopAllAsync();
        }
        public void SetPossibleThrows(int currentScore, int inputScore)
        {
            this.currentScore = currentScore;
            this.inputScore = inputScore;

            if (inputScore == 110 || inputScore == 107 || inputScore == 104 || inputScore == 101)
            {
                ThrowCount1IsPossible = false;
                ThrowCount2IsPossible = true;
                ThrowCount3IsPossible = true;
                return;
            }
            
            if (inputScore > 100)
            {
                ThrowCount1IsPossible = false;
                ThrowCount2IsPossible = false;
                ThrowCount3IsPossible = true;
            }
            else if ((inputScore <= 100 && inputScore > 50) || 
                     (inputScore < 50 && inputScore %2 == 1))
            {
                ThrowCount1IsPossible = false;
                ThrowCount2IsPossible = true;
                ThrowCount3IsPossible = true;
            }
            else
            {
                ThrowCount1IsPossible = true;
                ThrowCount2IsPossible = true;
                ThrowCount3IsPossible = true;
            }
        }

        public int GetTotalDartsThrown()
        {
            return selectedTotalThrows;
        }
    }
}