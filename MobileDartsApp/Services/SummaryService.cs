using MobileDartsApp.Services.Online.Dtos.Game;

namespace MobileDartsApp.Services
{
    public static class SummaryService
    {
        private static GameSummaryDto Summary = new();
        public static void Set(GameSummaryDto s) => Summary = s;
        public static GameSummaryDto Get () => Summary;
    }
}
