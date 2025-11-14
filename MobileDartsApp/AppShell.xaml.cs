
using MobileDartsApp.Views;
using System.Diagnostics;

namespace MobileDartsApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            registerRoutes();
        }

        private void registerRoutes()
        {
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(VictoryPage), typeof(VictoryPage));
            Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
            Routing.RegisterRoute(nameof(PlayerSelectorPage), typeof(PlayerSelectorPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(OnlineSettingsPage), typeof(OnlineSettingsPage));
            Routing.RegisterRoute(nameof(LobbyRoomPage), typeof(LobbyRoomPage));
            Routing.RegisterRoute(nameof(LobbySelectorPage), typeof(LobbySelectorPage));
            Routing.RegisterRoute(nameof(OnlineGamePage), typeof(OnlineGamePage));
            Routing.RegisterRoute(nameof(MatchSummaryPage), typeof(MatchSummaryPage));

        }
    }
}
