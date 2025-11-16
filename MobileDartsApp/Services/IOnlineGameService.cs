using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services
{
    public interface IOnlineGameService
    {
        event Action<ThrowAppliedDto> ThrowApplied;
        event Action<GameStateDto> StateChanged; 
        event Action<GameSummaryDto>? OnMatchEnded;

        bool IsConnected { get; }

        Task StartAsync(string lobbyGuid);
        Task StopAsync(string lobbyGuid);
        Task SubmitThrowAsync(string lobbyGuid, SubmitThrowDto dto);
        Task<GameStateDto?> GetStateAsync(string lobbyGuid);
        Task<GameSummaryDto?> GetSummaryAsync(string lobbyGuid);
        Task ForfeitAsync(string lobbyGuid);

    }
}
