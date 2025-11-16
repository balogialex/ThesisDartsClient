using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Services.Online.DataServices.SignalR;
using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services
{
    public class OnlineGameService : IOnlineGameService
    {
        private readonly IRestGameDataService _rest;
        private readonly ISignalRGameService _signalR;

        public bool IsConnected => _signalR.IsConnected;

        public event Action<ThrowAppliedDto>? ThrowApplied;
        public event Action<GameStateDto>? StateChanged; 
        public event Action<GameSummaryDto>? OnMatchEnded; 

        public OnlineGameService(IRestGameDataService rest, ISignalRGameService signalR)
        {
            _rest = rest;
            _signalR = signalR;

            _signalR.OnThrowApplied += dto => ThrowApplied?.Invoke(dto);
            _signalR.OnFullState += state => StateChanged?.Invoke(state);
            _signalR.OnMatchEnded += s => OnMatchEnded?.Invoke(s);
        }

        public async Task StartAsync(string lobbyGuid)
        {
            if (!IsConnected)
                await _signalR.StartConnectionAsync();

            await _signalR.JoinGameGroup(lobbyGuid);
        }

        public async Task StopAsync(string lobbyGuid)
        {
            await _signalR.LeaveGameGroup(lobbyGuid);
            await _signalR.StopConnectionAsync();
        }

        public Task SubmitThrowAsync(string lobbyGuid, SubmitThrowDto dto) =>
            _rest.SubmitThrowAsync(lobbyGuid, dto);

        public Task<GameStateDto?> GetStateAsync(string lobbyGuid) =>
            _rest.GetStateAsync(lobbyGuid);

        public Task<GameSummaryDto?> GetSummaryAsync(string lobbyGuid) =>
            _rest.GetSummaryAsync(lobbyGuid);

        public Task ForfeitAsync(string lobbyGuid) => 
            _rest.ForfeitAsync(lobbyGuid);

    }
}
