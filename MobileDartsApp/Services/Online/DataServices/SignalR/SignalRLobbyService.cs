using Microsoft.AspNetCore.SignalR.Client;
using MobileDartsApp.Services.Online.Dtos.Lobby;
using System.Diagnostics;

namespace MobileDartsApp.Services.Online.DataServices.SignalR
{
    public class SignalRLobbyService : SignalRHubServiceBase, ISignalRLobbyService
    {
        public event Action<LobbyDto>? OnPlayerJoined;
        public event Action<LobbyDto>? OnPlayerLeft;
        public event Action<LobbyDto>? OnGameStarted;
        public event Action<LobbyDto>? OnReadyStatusUpdated;
        public event Action<LobbyDto>? OnLobbyDeleted;
        public event Action<LobbyDto>? OnLobbyCreated;
        public event Action<LobbyDto>? OnLobbyClosed;

        public SignalRLobbyService()
            : base("lobbyHub")
        {
            _connection.On<LobbyDto>("PlayerJoined", lobby => OnPlayerJoined?.Invoke(lobby));
            _connection.On<LobbyDto>("ReadyStatusUpdated", lobby => OnReadyStatusUpdated?.Invoke(lobby));
            _connection.On<LobbyDto>("GameStarted", lobby => OnGameStarted?.Invoke(lobby));
            _connection.On<LobbyDto>("PlayerLeft", lobby => OnPlayerLeft?.Invoke(lobby));
            _connection.On<LobbyDto>("LobbyCreated", lobby => OnLobbyCreated?.Invoke(lobby));
            _connection.On<LobbyDto>("LobbyDeleted", lobby => OnLobbyDeleted?.Invoke(lobby));
            _connection.On<LobbyDto>("LobbyClosed", lobby => OnLobbyClosed?.Invoke(lobby));
        }

        public async Task JoinLobbyGroup(string lobbyGUID)
        {
            await _connection.InvokeAsync("JoinLobbyGroup", lobbyGUID);
            Debug.WriteLine($"Joined lobby group: {lobbyGUID}");
        }

        public async Task LeaveLobbyGroup(string lobbyGUID)
        {
            await _connection.InvokeAsync("LeaveLobbyGroup", lobbyGUID);
            Debug.WriteLine($"Left lobby group: {lobbyGUID}");
        }
    }
}
