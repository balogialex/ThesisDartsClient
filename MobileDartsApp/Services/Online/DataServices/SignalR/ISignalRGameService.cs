using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services.Online.DataServices.SignalR
{
    public interface ISignalRGameService
    {
        bool IsConnected { get; }

        event Action<ThrowAppliedDto> OnThrowApplied;
        event Action<GameStateDto> OnFullState;
        event Action<GameSummaryDto>? OnMatchEnded;

        event Action<bool> ConnectionStateChanged;

        Task StartConnectionAsync();
        Task StopConnectionAsync();

        Task JoinGameGroup(string lobbyGUID);
        Task LeaveGameGroup(string lobbyGUID);
    }
}
