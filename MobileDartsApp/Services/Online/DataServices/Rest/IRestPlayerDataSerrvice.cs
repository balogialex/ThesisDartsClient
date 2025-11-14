using MobileDartsApp.Services.Online.Dtos.Player;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public interface IRestPlayerDataService
    {
        Task<List<PlayerDto>> GetAllPlayersAsync();
        Task<(bool, string)> RegisterPlayerAsync(PlayerDto toDo);
        Task<(bool, string)> LoginPlayerAsync(PlayerDto toDo);
        Task<PlayerDto?> GetCurrentUserAsync();
        public void Logout();
    }
}
