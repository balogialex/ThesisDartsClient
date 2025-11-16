using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.DataServices.SignalR;

namespace MobileDartsApp
{
    public partial class App : Application
    {
        private readonly LocalGameDbService _localGameDbService;
        private readonly ISignalRLobbyService _signalRLobbyService;
        public App(LocalGameDbService gameDbService, ISignalRLobbyService signalRLobbyService)
        {
            InitializeComponent();

            _localGameDbService = gameDbService;
            InitializeDatabaseAsync();

            _signalRLobbyService = signalRLobbyService;
            ConnectToSignalRServer();

            MainPage = new AppShell();
        }
        private async void ConnectToSignalRServer()
        {
            await _signalRLobbyService.StartConnectionAsync();
        }
        private async void InitializeDatabaseAsync()
        {
            await _localGameDbService.InitializeTablesAsync();
        }
    }
}
