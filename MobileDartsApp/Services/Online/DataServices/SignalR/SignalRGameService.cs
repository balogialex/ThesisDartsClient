using Microsoft.AspNetCore.SignalR.Client;
using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services.Online.DataServices.SignalR
{
    public class SignalRGameService : SignalRHubServiceBase, ISignalRGameService
    {
        public event Action<ThrowAppliedDto>? OnThrowApplied;
        public event Action<GameStateDto>? OnFullState; 
        public event Action<GameSummaryDto>? OnMatchEnded;


        public new event Action<bool>? ConnectionStateChanged
        {
            add { base.ConnectionStateChanged += value; }
            remove { base.ConnectionStateChanged -= value; }
        }

        public SignalRGameService()
            : base("gameHub")
        {
            _connection.On<ThrowAppliedDto>("ThrowApplied", dto => OnThrowApplied?.Invoke(dto));
            _connection.On<GameStateDto>("FullState", state => OnFullState?.Invoke(state));
            _connection.On<GameSummaryDto>("MatchEnded", s => OnMatchEnded?.Invoke(s));

        }

        public Task JoinGameGroup(string lobbyGUID) =>
            _connection.InvokeAsync("JoinGameGroup", lobbyGUID);

        public Task LeaveGameGroup(string lobbyGUID) =>
            _connection.InvokeAsync("LeaveGameGroup", lobbyGUID);
    }
}
