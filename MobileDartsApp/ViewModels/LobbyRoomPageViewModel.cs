using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MobileDartsApp.Services.Online.Dtos.Lobby;
using MobileDartsApp.Services.Online.DataServices.SignalR;

namespace MobileDartsApp.ViewModels
{
    public class LobbyRoomPageViewModel : ObservableObject
    {
        #region Fields
        private readonly IRestLobbyDataService _restLobbyService;
        private readonly ISignalRLobbyService _signalRLobbyService;

        private LobbyDto _lobbyData = new LobbyDto();
        private string _currentUsername = string.Empty;
        private bool _isCreator;
        #endregion

        #region Properties
        public LobbyDto LobbyData
        {
            get => _lobbyData;
            set => SetProperty(ref _lobbyData, value);
        }
        public ObservableCollection<PlayerStatus> Players { get; set; } = new ObservableCollection<PlayerStatus>();
        public bool IsCreator
        {
            get => _isCreator;
            set => SetProperty(ref _isCreator, value);
        }
        #endregion

        #region Commands
        public IAsyncRelayCommand ToggleReadyCommand { get; }
        public IAsyncRelayCommand StartGameCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand LeaveLobbyCommand { get; set; }
        #endregion

        public LobbyRoomPageViewModel(IRestLobbyDataService lobbyService, ISignalRLobbyService signalRLobbyService)
        {
            _restLobbyService = lobbyService;
            _signalRLobbyService = signalRLobbyService;

            ToggleReadyCommand = new AsyncRelayCommand(ToggleReadyAsync);
            StartGameCommand = new AsyncRelayCommand(StartGameAsync);
            RefreshCommand = new AsyncRelayCommand(LoadLobbyDataAsync);
            LeaveLobbyCommand = new AsyncRelayCommand(LeaveLobbyAsync);

            subscribeToSignalRLobbyEvents();

            Task.Run(async () =>
            {
                _currentUsername = await SecureStorage.GetAsync("user_username");
                await LoadLobbyDataAsync();
            });
        }

        private async Task LeaveLobbyAsync()
        {
            try
            {
                string lobbyGUID = await SecureStorage.GetAsync("currentLobbyGUID");
                if (string.IsNullOrEmpty(lobbyGUID))
                {
                    await Shell.Current.DisplayAlert("Error", "No lobby GUID found.", "OK");
                    return;
                }

                await _restLobbyService.LeaveLobbyAsync(lobbyGUID);

                unsubscribeFromSignalRLobbyEvents();

                await Shell.Current.Navigation.PopToRootAsync();
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error leaving lobby: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to leave lobby: {ex.Message}", "OK");
            }
        }
        private async Task LoadLobbyDataAsync()
        {
            try
            {
                string lobbyGUID = await SecureStorage.GetAsync("currentLobbyGUID");
                if (string.IsNullOrEmpty(lobbyGUID))
                {
                    await Shell.Current.DisplayAlert("Error", "No lobby GUID found.", "OK");
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                LobbyData = await _restLobbyService.GetLobbyStatusAsync(lobbyGUID);
                if (LobbyData == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load lobby data.", "OK");
                    return;
                }

                IsCreator = LobbyData.LobbyCreator == _currentUsername;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Players.Clear();
                    foreach (var player in LobbyData.Players)
                    {
                        Players.Add(new PlayerStatus
                        {
                            PlayerName = player,
                            IsReady = LobbyData.PlayerReadiness.GetValueOrDefault(player, false)
                        });
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading lobby data: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to load lobby: {ex.Message}", "OK");
            }
        }
        private async Task ToggleReadyAsync()
        {
            try
            {
                string lobbyGUID = await SecureStorage.GetAsync("currentLobbyGUID");
                if (string.IsNullOrEmpty(lobbyGUID))
                {
                    await Shell.Current.DisplayAlert("Error", "No lobby GUID found.", "OK");
                    return;
                }

                await _restLobbyService.UpdateReadyStatusAsync(lobbyGUID);
                await LoadLobbyDataAsync();
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling ready status: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to update ready status: {ex.Message}", "OK");
            }
        }
        private async Task StartGameAsync()
        {
            try
            {
                string lobbyGUID = await SecureStorage.GetAsync("currentLobbyGUID");
                if (string.IsNullOrEmpty(lobbyGUID))
                {
                    await Shell.Current.DisplayAlert("Error", "No lobby GUID found.", "OK");
                    return;
                }

                await _restLobbyService.StartGameAsync(lobbyGUID);
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting game: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to start game: {ex.Message}", "OK");
            }
        }

        #region SignalR event handlers

        private async void OnPlayerJoinedHandler(LobbyDto lobby)
        {
            if (lobby.LobbyGUID == LobbyData?.LobbyGUID)
            {
                await LoadLobbyDataAsync();
            }
        }

        private async void OnPlayerLeftHandler(LobbyDto lobby)
        {
            if (lobby.LobbyGUID == LobbyData?.LobbyGUID)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Players.Clear();
                    foreach (var player in lobby.Players)
                    {
                        Players.Add(new PlayerStatus
                        {
                            PlayerName = player,
                            IsReady = lobby.PlayerReadiness.GetValueOrDefault(player, false)
                        });
                    }
                });
            }
        }

        private async void OnLobbyDeletedHandler(LobbyDto lobby)
        {
            if (lobby.LobbyGUID == LobbyData?.LobbyGUID)
            {
                SecureStorage.Remove("currentLobbyGUID");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.Navigation.PopToRootAsync();
                    await Shell.Current.DisplayAlert("Info", "The lobby has been deleted.", "OK");
                });
            }
        }

        private async void OnReadyStatusUpdatedHandler(LobbyDto lobby)
        {
            if (lobby.LobbyGUID != LobbyData?.LobbyGUID)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Players.Clear();
                foreach (var player in lobby.Players)
                {
                    Players.Add(new PlayerStatus
                    {
                        PlayerName = player,
                        IsReady = lobby.PlayerReadiness.GetValueOrDefault(player, false)
                    });
                }
            });
        }

        private async void OnGameStartedHandler(LobbyDto lobby)
        {
            if (lobby.LobbyGUID == LobbyData?.LobbyGUID)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync(nameof(OnlineGamePage));
                });
            }
        }
        #endregion

        private void subscribeToSignalRLobbyEvents()
        {
            _signalRLobbyService.OnPlayerJoined += OnPlayerJoinedHandler;

            _signalRLobbyService.OnPlayerLeft += OnPlayerLeftHandler;

            _signalRLobbyService.OnLobbyDeleted += OnLobbyDeletedHandler;

            _signalRLobbyService.OnReadyStatusUpdated += OnReadyStatusUpdatedHandler;

            _signalRLobbyService.OnGameStarted += OnGameStartedHandler;
        }
        private void unsubscribeFromSignalRLobbyEvents()
        {
            _signalRLobbyService.OnPlayerJoined -= OnPlayerJoinedHandler;

            _signalRLobbyService.OnPlayerLeft -= OnPlayerLeftHandler;

            _signalRLobbyService.OnLobbyDeleted -= OnLobbyDeletedHandler;

            _signalRLobbyService.OnReadyStatusUpdated -= OnReadyStatusUpdatedHandler;

            _signalRLobbyService.OnGameStarted -= OnGameStartedHandler;
        }
    }


    public class PlayerStatus
    {
        public string PlayerName { get; set; }
        public bool IsReady { get; set; }
    }
}