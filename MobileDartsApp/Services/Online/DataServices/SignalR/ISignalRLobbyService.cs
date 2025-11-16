using MobileDartsApp.Services.Online.Dtos.Lobby;

namespace MobileDartsApp.Services.Online.DataServices.SignalR
{
    public interface ISignalRLobbyService
    {
        bool IsConnected { get; }

        event Action<LobbyDto> OnPlayerJoined;
        event Action<LobbyDto> OnPlayerLeft;
        event Action<LobbyDto> OnGameStarted;
        event Action<LobbyDto> OnReadyStatusUpdated;
        event Action<LobbyDto> OnLobbyDeleted;
        event Action<LobbyDto> OnLobbyCreated;
        event Action<LobbyDto> OnLobbyClosed;

        event Action<bool> ConnectionStateChanged;

        Task StartConnectionAsync();
        Task StopConnectionAsync();
        Task JoinLobbyGroup(string lobbyGUID);
        Task LeaveLobbyGroup(string lobbyGUID);



    }
}
