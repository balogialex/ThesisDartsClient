using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Models;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Services.Online.DataServices.SignalR;
using MobileDartsApp.Views;
using System.Diagnostics;

namespace MobileDartsApp.ViewModels
{
    public class OnlineSettingsViewModel : ObservableObject
    {
        #region fields
        private int selectedNumberOfLegs = 1;
        private LegFormat selectedLegFormat;
        private int selectedGameMode;
        private readonly IRestLobbyDataService _restLobbyService;
        private readonly ISignalRLobbyService _signalRLobbyService;
        private readonly SettingsService _settingsService;
        #endregion

        #region properties
        public int SelectedNumberOfLegs
        {
            get => selectedNumberOfLegs;
            set
            {
                if (selectedNumberOfLegs != value)
                {
                    selectedNumberOfLegs = value;
                    OnPropertyChanged(nameof(SelectedNumberOfLegs));
                }
            }
        }

        public int SelectedGameMode
        {
            get => selectedGameMode;
            set
            {
                if (selectedGameMode != value)
                {
                    selectedGameMode = value;
                    OnPropertyChanged(nameof(SelectedGameMode));
                }
            }
        }

        public LegFormat SelectedLegFormat
        {
            get => selectedLegFormat;
            set
            {
                if (selectedLegFormat != value)
                {
                    selectedLegFormat = value;
                    OnPropertyChanged(nameof(SelectedLegFormat));
                }
            }
        }
        #endregion

        #region commands
        public IAsyncRelayCommand CreateLobby { get; }
        public IRelayCommand MinusLeg { get; }
        public IRelayCommand PlusLeg { get; }
        public IRelayCommand<object> SelectGamemode { get; }
        public IRelayCommand<object> SelectLegFormat { get; }
        #endregion

        public OnlineSettingsViewModel(
            SettingsService settingsService,
            ISignalRLobbyService signalRLobbyService,
            IRestLobbyDataService lobbyService)
        {
            _settingsService = settingsService;
            _restLobbyService = lobbyService;
            _signalRLobbyService = signalRLobbyService;

            selectedNumberOfLegs = 3;
            SelectedGameMode = 501;
            SelectedLegFormat = LegFormat.FirstTo;

            CreateLobby = new AsyncRelayCommand(CreateLobbyExecuteAsync);
            PlusLeg = new RelayCommand(plusLegExecute);
            MinusLeg = new RelayCommand(minusLegExecute);
            SelectGamemode = new RelayCommand<object>(OnGamemodeSelected);
            SelectLegFormat = new RelayCommand<object>(OnLegFormatSelected);
        }

        private void minusLegExecute()
        {
            if (SelectedNumberOfLegs > 1)
                SelectedNumberOfLegs -= 1;
        }

        private void plusLegExecute()
        {
            if (SelectedNumberOfLegs < 9)
                SelectedNumberOfLegs += 1;
        }

        private void OnGamemodeSelected(object obj)
        {
            if (obj is null) return;
            if (int.TryParse(obj.ToString(), out var gm))
                SelectedGameMode = gm;
        }

        private void OnLegFormatSelected(object obj)
        {
            var s = obj?.ToString()?.ToLowerInvariant();
            SelectedLegFormat = (s == "firstto") ? LegFormat.FirstTo : LegFormat.BestOf;
        }

        private async Task CreateLobbyExecuteAsync()
        {
            try
            {
                var settings = new GameSettingsModel
                {
                    LegCount = SelectedNumberOfLegs,
                    MatchFormat = MatchFormat.Leg,
                    LegFormat = SelectedLegFormat,
                    StartScore = SelectedGameMode,
                    OrderOfPlayers = new List<string>()
                };

                var username = await SecureStorage.GetAsync("user_username");
                if (!string.IsNullOrWhiteSpace(username))
                    settings.OrderOfPlayers.Add(username);

                _settingsService.LoadOnlineSettings(settings);

                var lobbyDto = await _restLobbyService.CreateLobby();
                if (lobbyDto == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to create lobby.", "OK");
                    return;
                }

                await SecureStorage.SetAsync("currentLobbyGUID", lobbyDto.LobbyGUID);
                await _signalRLobbyService.JoinLobbyGroup(lobbyDto.LobbyGUID);

                await Shell.Current.GoToAsync(nameof(LobbyRoomPage));
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating lobby: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to create lobby: {ex.Message}", "OK");
            }
        }
    }
}
