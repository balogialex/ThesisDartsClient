namespace MobileDartsApp.Services
{
    using MobileDartsApp.Models;

    public class SettingsService
    {
        public GameSettingsModel CurrentSettings { get; private set; }

        public void LoadLocalSettings(LocalGameSettings localSettings)
        {
            CurrentSettings = localSettings;
        }

        public void LoadOnlineSettings(GameSettingsModel onlineSettings)
        {
            CurrentSettings = onlineSettings;
        }
    }
}
