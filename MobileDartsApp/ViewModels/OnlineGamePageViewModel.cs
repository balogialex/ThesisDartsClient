using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.Dtos.Game;
using MobileDartsApp.Views;
using Mopups.Services;

namespace MobileDartsApp.ViewModels
{
    public class OnlineGamePageViewModel : ObservableObject
    {
        #region fields
        private readonly CheckoutPopupPage _checkoutPage;
        private readonly DoubleTriesPopupPage _doubleTriesPage;
        private readonly HashSet<int> NotPossibleCheckouts = new() { 180, 177, 174, 171, 169, 168, 166, 165, 163, 162, 159 };
        private readonly IOnlineGameService _onlineGameService;
        private string lobbyGuid = string.Empty;
        private string myName = string.Empty;
        private string playerName = string.Empty;
        private int currentScore;
        private string insertedScore = string.Empty;
        private bool isMyTurn;
        private bool isSubmitting;
        private string opponentName = string.Empty;
        private int opponentScore;
        #endregion

        #region properties  
        public string LobbyGuid
        {
            get => lobbyGuid;
            set => SetProperty(ref lobbyGuid, value);
        }
        public string MyName
        {
            get => myName;
            set => SetProperty(ref myName, value);
        }
        public string PlayerName
        {
            get => playerName;
            set => SetProperty(ref playerName, value);
        }
        public int CurrentScore
        {
            get => currentScore;
            set => SetProperty(ref currentScore, value);
        }
        public string InsertedScore
        {
            get => insertedScore;
            set => SetProperty(ref insertedScore, value);
        }
        public bool IsMyTurn
        {
            get => isMyTurn;
            set => SetProperty(ref isMyTurn, value);
        }
        public bool IsSubmitting
        {
            get => isSubmitting;
            set => SetProperty(ref isSubmitting, value);
        }
        public string OpponentName
        {
            get => opponentName;
            set => SetProperty(ref opponentName, value);
        }
        public int OpponentScore
        {
            get => opponentScore;
            set => SetProperty(ref opponentScore, value);
        }
        #endregion

        #region commands
        public IRelayCommand<object> InsertNumber { get; }
        public IAsyncRelayCommand OkPressed { get; }
        public IRelayCommand Backspace { get; }
        public IRelayCommand Clear { get; }
        public IAsyncRelayCommand ForfeitCommand { get; }
        #endregion

        public OnlineGamePageViewModel(IOnlineGameService online, CheckoutPopupPage checkoutPopup, DoubleTriesPopupPage doubleTriesPopup)
        {
            _onlineGameService = online;
            _checkoutPage = checkoutPopup;
            _doubleTriesPage = doubleTriesPopup;

            InsertNumber = new RelayCommand<object>(OnInsertNumber);
            Backspace = new RelayCommand(OnBackspace);
            Clear = new RelayCommand(() => InsertedScore = string.Empty);
            OkPressed = new AsyncRelayCommand(OnOkAsync);
            ForfeitCommand = new AsyncRelayCommand(forfeitAsync);

            _onlineGameService.ThrowApplied += OnThrowApplied;
            _onlineGameService.StateChanged += ApplyState;
            _onlineGameService.OnMatchEnded += OnMatchEnded;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            LobbyGuid = await SecureStorage.GetAsync("currentLobbyGUID") ?? "";
            MyName = await SecureStorage.GetAsync("user_username") ?? "";

            if (string.IsNullOrEmpty(LobbyGuid))
            {
                await Shell.Current.DisplayAlert("Hiba", "Hiányzik a lobby GUID.", "OK");
                await Shell.Current.Navigation.PopAsync();
                return;
            }

            await _onlineGameService.StartAsync(LobbyGuid);

            var state = await _onlineGameService.GetStateAsync(LobbyGuid);
            if (state != null) ApplyState(state);
        }

        private void ApplyState(GameStateDto state)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayerName = MyName;
                IsMyTurn = !string.IsNullOrEmpty(MyName) && MyName == state.CurrentPlayer;

                CurrentScore = state.Scores.TryGetValue(MyName, out var mine) ? mine : 0;

                var other = state.Scores.Keys.FirstOrDefault(n => n != MyName);
                if (!string.IsNullOrEmpty(other))
                {
                    OpponentName = other;
                    OpponentScore = state.Scores.TryGetValue(other, out var os) ? os : 0;
                }
                else
                {
                    OpponentName = string.Empty;
                    OpponentScore = 0;
                }
            });
        }

        private void OnThrowApplied(ThrowAppliedDto dto)
        {
            if (dto.LobbyGUID != LobbyGuid) return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (dto.PlayerName == MyName) CurrentScore = dto.NewScore;
                else OpponentScore = dto.NewScore;

                InsertedScore = string.Empty;

                if (dto.LegEnded && !dto.MatchEnded)
                {
                    await Shell.Current.DisplayAlert("Leg vége", $"{dto.PlayerName} kiszállt.", "OK");
                    await RefreshStateAsync();
                    return;
                }

                if (dto.MatchEnded)
                {
                    return;
                }

                await RefreshStateAsync();
            });
        }

        private async Task RefreshStateAsync()
        {
            var state = await _onlineGameService.GetStateAsync(LobbyGuid);
            if (state != null) ApplyState(state);
        }

        private void OnInsertNumber(object? param)
        {
            if (param is null) return;
            var s = param.ToString();
            if (string.IsNullOrWhiteSpace(s)) return;

            var candidate = (InsertedScore + s).Trim();
            if (!int.TryParse(candidate, out var num)) return;
            if (num < 0 || num > 180) return;

            InsertedScore = candidate;
        }

        private void OnBackspace()
        {
            if (!string.IsNullOrEmpty(InsertedScore))
                InsertedScore = InsertedScore[..^1];
        }

        private async Task OnOkAsync()
        {
            if (!IsMyTurn)
            {
                await Shell.Current.DisplayAlert("Várj", "Most nem a te köröd.", "OK");
                return;
            }

            if (!int.TryParse(InsertedScore, out var input)) return;
            var possibleScore = CurrentScore - input;

            int doubleTries = 0;
            int usedDarts = 3;

            if (possibleScore < 0 || possibleScore == 1) return;

            if (IsSubmitting) return;
            IsSubmitting = true;

            if (possibleScore <= 50 && possibleScore >= 0)
            {
                await MopupService.Instance.PushAsync(_doubleTriesPage);
                _doubleTriesPage.ViewModel.SetPossibleDoubleTries(CurrentScore, input);

                while (MopupService.Instance.PopupStack.Count > 0)
                    await Task.Delay(200);

                doubleTries = _doubleTriesPage.ViewModel.GetDoubleTries();
            }

            if (possibleScore == 0)
            {
                await MopupService.Instance.PushAsync(_checkoutPage);
                _checkoutPage.ViewModel.SetPossibleThrows(CurrentScore, input);

                while (MopupService.Instance.PopupStack.Count > 0)
                    await Task.Delay(200);

                usedDarts = _checkoutPage.ViewModel.GetTotalDartsThrown();
            }

            var dto = new SubmitThrowDto
            {
                PlayerName = MyName,
                InputScore = input,
                DoubleTries = doubleTries,
                UsedDarts = usedDarts
            };

            try
            {
                await _onlineGameService.SubmitThrowAsync(LobbyGuid, dto);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK");
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private async Task forfeitAsync()
        {
            bool wantsToLeave = await Shell.Current.DisplayAlert("Kilépés", "Biztosan ki szeretne lépni?", "Igen", "Nem");
            if (wantsToLeave)
                await _onlineGameService.ForfeitAsync(LobbyGuid);
        }

        private async void OnMatchEnded(GameSummaryDto s)
        {
            if (s.LobbyGUID != LobbyGuid) return;

            SummaryService.Set(s);

            if (string.Equals(s.EndReason, "Forfeit", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(s.ForfeitedBy, MyName, StringComparison.Ordinal))
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Feladás", $"{s.ForfeitedBy} feladta a meccset.", "OK");
                });
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync(nameof(MatchSummaryPage));
            });
        }

    }
}
