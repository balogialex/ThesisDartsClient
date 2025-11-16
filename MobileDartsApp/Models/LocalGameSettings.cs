namespace MobileDartsApp.Models
{
    public class LocalGameSettings : GameSettingsModel
    {
        public List<PlayerModel> Players { get; set; }

        public LocalGameSettings()
        {
            Players = new List<PlayerModel>();
        }
    }
}
