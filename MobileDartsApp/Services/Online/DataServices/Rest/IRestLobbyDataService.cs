using MobileDartsApp.Services.Online.Dtos.Lobby;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public interface IRestLobbyDataService
    {
        Task<List<LobbyDto>> GetAllLobbies();

        Task<LobbyDto> JoinLobby(string lobbyGUID);

        Task<LobbyDto> CreateLobby();

        Task<LobbyDto?> GetLobbyStatusAsync(string lobbyGUID);

        Task UpdateReadyStatusAsync(string lobbyGUID);

        Task StartGameAsync(string lobbyGUID);

        Task LeaveLobbyAsync(string lobbyGUID);
    }
}
