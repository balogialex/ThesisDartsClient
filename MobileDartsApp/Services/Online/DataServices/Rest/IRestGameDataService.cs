using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public interface IRestGameDataService
    {
        Task<ThrowAppliedDto?> SubmitThrowAsync(string lobbyGUID, SubmitThrowDto dto);
        Task<GameStateDto?> GetStateAsync(string lobbyGUID);
        Task<GameSummaryDto?> GetSummaryAsync(string lobbyGUID);
        Task<bool> ForfeitAsync(string lobbyGUID);

    }
}
