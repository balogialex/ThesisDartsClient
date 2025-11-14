using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.ViewModels
{
    public class MatchSummaryPageViewModel : ObservableObject
    {
        #region fields
        private readonly IOnlineGameService _online;

        private string _lobbyGuid = string.Empty;
        private string _winner = string.Empty;

        private Dictionary<string, int> _legsWon = new();
        private Dictionary<string, int> _totalPoints = new();
        private Dictionary<string, int> _totalThrows = new();
        private Dictionary<string, double> _averagePerVisit = new();
        private Dictionary<string, int> _checkoutAttempts = new();
        private Dictionary<string, int> _checkouts = new();
        private Dictionary<string, int> _checkoutPercent = new();
        private Dictionary<string, string> _checkoutString = new();

        private bool isForfeit;
        private string? forfeitedBy = string.Empty;
        #endregion

        #region properties
        public string LobbyGuid
        {
            get => _lobbyGuid;
            set => SetProperty(ref _lobbyGuid, value);
        }
        public string Winner
        {
            get => _winner;
            set => SetProperty(ref _winner, value);
        }
        public Dictionary<string, int> LegsWon{
            get => _legsWon;
            set => SetProperty(ref _legsWon, value);
        }
        public Dictionary<string, int> TotalPoints
        {
            get => _totalPoints;
            set => SetProperty(ref _totalPoints, value);
        }
        public Dictionary<string, int> TotalThrows
        {
            get => _totalThrows;
            set => SetProperty(ref _totalThrows, value);
        }
        public Dictionary<string, double> AveragePerVisit
        {
            get => _averagePerVisit;
            set => SetProperty(ref _averagePerVisit, value);
        }
        public Dictionary<string, int> CheckoutAttempts
        {
            get => _checkoutAttempts;
            set => SetProperty(ref _checkoutAttempts, value);
        }
        public Dictionary<string, int> Checkouts
        {
            get => _checkouts;
            set => SetProperty(ref _checkouts, value);
        }
        public Dictionary<string, int> CheckoutPercent
        {
            get => _checkoutPercent;
            set => SetProperty(ref _checkoutPercent, value);
        }
        public Dictionary<string, string> CheckoutString
        {
            get => _checkoutString;
            set => SetProperty(ref _checkoutString, value);
        }
        public string? ForfeitedBy 
        {
            get => forfeitedBy;
            set => SetProperty(ref forfeitedBy, value);
        }
        public bool IsForfeit
        {
            get => isForfeit;
            set => SetProperty(ref isForfeit, value);
        }
        #endregion

        #region commands 
        public IAsyncRelayCommand NavigateToRootCommand { get; }
        #endregion

        public MatchSummaryPageViewModel(IOnlineGameService online)
        {
            _online = online;
            NavigateToRootCommand = new AsyncRelayCommand(NavigateToRootExecute);
        }
        public async Task LoadAsync()
        {
            if (string.IsNullOrEmpty(LobbyGuid))
                LobbyGuid = await SecureStorage.GetAsync("currentLobbyGUID") ?? "";

            GameSummaryDto? summary = SummaryService.Get();
            if (summary is null) return;

            Winner = summary.Winner;
            LegsWon = summary.LegsWon;
            TotalPoints = summary.TotalPoints;
            TotalThrows = summary.TotalThrows;
            AveragePerVisit = summary.Average;
            CheckoutAttempts = summary.CheckoutAttempts;
            Checkouts = summary.Checkouts;
            CheckoutPercent = summary.CheckoutPercent;
            var cs = new Dictionary<string, string>();
            foreach (var item in Checkouts)
            {
                var success = item.Value;
                var attempts = CheckoutAttempts.TryGetValue(item.Key, out var att) ? att : 0;
                cs[item.Key] = $"{success} / {attempts}";
            }
            CheckoutString = cs;

            IsForfeit = string.Equals(summary.EndReason, "Forfeit", StringComparison.OrdinalIgnoreCase);
            ForfeitedBy = summary.ForfeitedBy;
        }

        private async Task NavigateToRootExecute()
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}
