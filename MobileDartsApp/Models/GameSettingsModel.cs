namespace MobileDartsApp.Models
{
    public enum LegFormat
    {
        FirstTo,
        BestOf
    }

    public enum MatchFormat
    {
        // TODO: implement Set format
        Leg
    }

    public class GameSettingsModel
    {
        public MatchFormat MatchFormat { get; set; }
        public LegFormat LegFormat { get; set; }
        public int LegCount { get; set; }
        public int StartScore { get; set; }

        public List<string> OrderOfPlayers { get; set; }

        public GameSettingsModel()
        {
            OrderOfPlayers = new List<string>();
        }
    }
}
