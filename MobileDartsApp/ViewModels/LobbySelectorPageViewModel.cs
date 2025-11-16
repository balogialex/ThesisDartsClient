using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MobileDartsApp.Services.Online.Dtos.Lobby;
using MobileDartsApp.Services.Online.DataServices.SignalR;

namespace MobileDartsApp.ViewModels
{
    public partial class LobbySelectorPageViewModel : ObservableObject
    {
        #region Fields
        private readonly IRestLobbyDataService _lobbyService;
        private readonly SettingsService _settingsService;
        private readonly ISignalRLobbyService _signalRLobbyService;
        private bool _isConnected;
        private bool _isBusy;
        private bool _isRefreshing;
        #endregion

        #region Handlers
        private Action<bool>? _connectionChangedHandler;
        private Action<LobbyDto>? _onLobbyCreatedHandler;
        private Action<LobbyDto>? _onLobbyClosedHandler;
        #endregion

        #region Properties
        public ObservableCollection<LobbyDto> Lobbies { get; set; } = new();

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        #endregion

        #region Commands
        public ICommand CreateLobbyCommand { get; }
        public IAsyncRelayCommand<LobbyDto> JoinLobbyCommand { get; }
        public IAsyncRelayCommand ConnectToSignalRServer { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        #endregion

        public LobbySelectorPageViewModel(IRestLobbyDataService lobbyService, 
            ISignalRLobbyService signalRLobbyService, 
            SettingsService settingsService)
        {
            _lobbyService = lobbyService;
            _signalRLobbyService = signalRLobbyService;
            _settingsService = settingsService;

            IsConnected = _signalRLobbyService.IsConnected;

            ConnectToSignalRServer = new AsyncRelayCommand(connectToServerAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            JoinLobbyCommand = new AsyncRelayCommand<LobbyDto>(joinLobbyAsync);
            CreateLobbyCommand = new AsyncRelayCommand(createLobbyAsync);
        }

        private async Task RefreshAsync()
        {
            try
            {
                if (!IsConnected) return;
                await LoadLobbiesAsync();
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsRefreshing = false);
            }
        }

        private async Task connectToServerAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _signalRLobbyService.StartConnectionAsync();

                IsConnected = _signalRLobbyService.IsConnected;

                if (!IsConnected)
                {
                    await Shell.Current.DisplayAlert(
                        "Nem sikerült csatlakozni",
                        "A szerver jelenleg nem elérhető. Később próbáld újra.",
                        "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadLobbiesAsync()
        {
            if (!IsConnected) return;
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var lobbyList = await _lobbyService.GetAllLobbies();
                Lobbies.Clear();
                foreach (var lobby in lobbyList)
                {
                    Lobbies.Add(lobby);
                }
            }
            catch (UnauthorizedAccessException)
            {
                //await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load lobbies: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task createLobbyAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(OnlineSettingsPage));
            }
            catch (UnauthorizedAccessException)
            {
                // await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to create lobby: {ex.Message}", "OK");
            }
        }

        private async Task joinLobbyAsync(LobbyDto lobby)
        {
            if (lobby is null) return;

            try
            {
                var joinedLobby = await _lobbyService.JoinLobby(lobby.LobbyGUID);
                if (joinedLobby == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to join lobby.", "OK");
                    return;
                }

                await _signalRLobbyService.JoinLobbyGroup(lobby.LobbyGUID);

                await SecureStorage.SetAsync("currentLobbyGUID", joinedLobby.LobbyGUID);
                await Shell.Current.GoToAsync(nameof(LobbyRoomPage));
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to join lobby: {ex.Message}", "OK");
            }
        }

        #region Event Subscription handling
        public async Task OnAppearingAsync()
        {
            SubscribeEventsIfNeeded();

            IsConnected = _signalRLobbyService.IsConnected;

            if (!IsConnected)
                await connectToServerAsync();
            else
                await LoadLobbiesAsync();
        }

        public Task OnDisappearingAsync()
        {
            UnsubscribeEventsIfNeeded();
            return Task.CompletedTask;
        }

        private void SubscribeEventsIfNeeded()
        {
            if (_connectionChangedHandler != null) return;

            _connectionChangedHandler = async connected =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    IsConnected = connected;
                    if (connected)
                    {
                        if (!IsBusy && !IsRefreshing)
                            await LoadLobbiesAsync();
                    }
                    else
                    {
                        Lobbies.Clear();
                    }
                });
            };


            _onLobbyCreatedHandler = lobby =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!Lobbies.Any(x => x.LobbyGUID == lobby.LobbyGUID))
                        Lobbies.Add(lobby);
                });
            };

            _onLobbyClosedHandler = closedLobby =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var existing = Lobbies.FirstOrDefault(l => l.LobbyGUID == closedLobby.LobbyGUID);
                    if (existing != null) Lobbies.Remove(existing);
                });
            };

            _signalRLobbyService.ConnectionStateChanged += _connectionChangedHandler;
            _signalRLobbyService.OnLobbyCreated += _onLobbyCreatedHandler;
            _signalRLobbyService.OnLobbyClosed += _onLobbyClosedHandler;
        }

        private void UnsubscribeEventsIfNeeded()
        {
            if (_connectionChangedHandler == null) return;

            _signalRLobbyService.ConnectionStateChanged -= _connectionChangedHandler;
            _signalRLobbyService.OnLobbyCreated -= _onLobbyCreatedHandler!;
            _signalRLobbyService.OnLobbyClosed -= _onLobbyClosedHandler!;

            _connectionChangedHandler = null;
            _onLobbyCreatedHandler = null;
            _onLobbyClosedHandler = null;
        }
        #endregion
    }
}
