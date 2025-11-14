using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Views;
using Mopups.Services;

namespace MobileDartsApp.ViewModels
{
    public class GamePageViewModel : ObservableObject
    {
        private readonly GameService _gameService;

        #region fields 
        private DoubleTriesPopupPage _doubleTriesPopup;
        private CheckoutPopupPage _checkoutPopup;
        private string insertedScore = string.Empty;
        private int currentScore;
        private string _playerName;
        #endregion

        #region Properties
        public int CurrentScore
        {
            get => currentScore;
            set
            {
                if (currentScore != value)
                {
                    currentScore = value;
                    OnPropertyChanged(nameof(CurrentScore));
                }
            }
        }

        public string PlayerName
        {
            get => _playerName;
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                    OnPropertyChanged(nameof(PlayerName));
                }
            }
        }

        public string InsertedScore
        {
            get => insertedScore;
            set
            {
                if (insertedScore != value)
                {
                    insertedScore = value;
                    OnPropertyChanged(nameof(InsertedScore));
                }
            }
        }

        public double AverageScore => _gameService.GetAverageForPlayer(PlayerName);
        public double CheckoutPercent => _gameService.GetCheckoutPercentForPlayer(PlayerName);
        #endregion

        #region Commands
        public IRelayCommand<object> InsertNumber { get; }
        public IAsyncRelayCommand OKPressed { get; }
        public IRelayCommand<object> BackPressed { get; }
        public IAsyncRelayCommand ForfeitCommand {get; }
        #endregion
        public GamePageViewModel(GameService gameService, DoubleTriesPopupPage doubleTriesPopupPage, CheckoutPopupPage checkoutPopupPage)
        {
            _doubleTriesPopup = doubleTriesPopupPage;
            _checkoutPopup = checkoutPopupPage;

            _gameService = gameService;
            _gameService.CreateNewGame();

            InsertNumber = new RelayCommand<object>(InsertNumberExecute);
            OKPressed = new AsyncRelayCommand(OkPressedAsync);
            BackPressed = new RelayCommand<object>(BackPressedExecute);
            ForfeitCommand = new AsyncRelayCommand(forfeitAsync);

            LoadNextThrow();
        }
        private async Task forfeitAsync()
        {
            bool wantsToLeave = await Shell.Current.DisplayAlert("Kilépés", "Biztosan ki szeretne lépni?", "Igen", "Nem");
            if (wantsToLeave)
                await Shell.Current.Navigation.PopAsync();
        }
        private void BackPressedExecute(object _)
        {
            var result = _gameService.UndoOrBackspace(InsertedScore);

            InsertedScore = result.InsertedScore;

            if (!result.HadInsertedScore && !string.IsNullOrEmpty(result.PlayerName))
            {
                PlayerName = result.PlayerName;
                CurrentScore = result.CurrentScore;
                UpdateStatsOnUI();
            }
        }

        private async Task OkPressedAsync()
        {
            // 1) Előkészítés
            var flow = _gameService.PrepareTurn(InsertedScore, PlayerName, CurrentScore);
            if (!flow.IsValid) return;

            // 2) Popupok (ha kell)
            if (flow.RequiresDoubleTries)
            {
                var doubleTries = await ShowDoubleTriesPopupAsync(int.Parse(InsertedScore), CurrentScore);
                _gameService.SetPendingDoubleTries(doubleTries);
            }

            if (flow.RequiresCheckoutInfo)
            {
                var dartsUsed = await ShowCheckoutPopupAsync(int.Parse(InsertedScore), CurrentScore);
                _gameService.SetPendingCheckoutDarts(dartsUsed);
            }

            // 3) Commit → itt már a saját régi TurnResult-odat használjuk
            var result = _gameService.CommitTurn();
            InsertedScore = string.Empty;

            if (!result.IsValid) return;

            if (result.MatchEnded)
            {
                await Shell.Current.GoToAsync(nameof(VictoryPage));
                return;
            }

            // Friss UI
            LoadNextThrow();
            UpdateStatsOnUI();
        }

        private void LoadNextThrow()
        {
            var tm = _gameService.GetNextThrowData();
            PlayerName = tm.PlayerName;
            CurrentScore = tm.CurrentScore - tm.Scored; // dobás ELŐTTI állapot
        }

        private void UpdateStatsOnUI()
        {
            OnPropertyChanged(nameof(CheckoutPercent));
            OnPropertyChanged(nameof(AverageScore));
        }

        private void InsertNumberExecute(object number)
        {
            if (number is null) return;
            InsertedScore = _gameService.InsertNumber(InsertedScore, number.ToString()!, CurrentScore);
        }

        private async Task<int> ShowDoubleTriesPopupAsync(int inputScore, int currentScore)
        {
            await MopupService.Instance.PushAsync(_doubleTriesPopup);
            _doubleTriesPopup.ViewModel.SetPossibleDoubleTries(currentScore, inputScore);

            while (MopupService.Instance.PopupStack.Count > 0)
                await Task.Delay(200);

            return _doubleTriesPopup.ViewModel.GetDoubleTries();
        }

        private async Task<int> ShowCheckoutPopupAsync(int inputScore, int currentScore)
        {
            await MopupService.Instance.PushAsync(_checkoutPopup);
            _checkoutPopup.ViewModel.SetPossibleThrows(currentScore, inputScore);

            while (MopupService.Instance.PopupStack.Count > 0)
                await Task.Delay(200);

            return _checkoutPopup.ViewModel.GetTotalDartsThrown();
        }
    }
}
