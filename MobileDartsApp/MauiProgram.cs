using Microsoft.Extensions.Logging;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Services.Online.DataServices.SignalR;
using MobileDartsApp.ViewModels;
using MobileDartsApp.Views;
using Mopups.Hosting;

namespace MobileDartsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<CheckoutPopupPage>();
            builder.Services.AddTransient<DoubleTriesPopupPage>();
            builder.Services.AddTransient<VictoryPage>();
            builder.Services.AddTransient<GameSelectorPage>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<PlayerSelectorPage>();
            builder.Services.AddTransient<CreateNewPlayerPopupPage>();
            builder.Services.AddTransient<OnlineGamePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<LobbySelectorPage>();
            builder.Services.AddTransient<OnlineSettingsPage>();
            builder.Services.AddTransient<LobbyRoomPage>(); 
            builder.Services.AddTransient<MatchSummaryPage>();


            builder.Services.AddTransient<GamePageViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();
            builder.Services.AddTransient<VictoryPageViewModel>();
            builder.Services.AddTransient<GameSelectorPageViewModel>();
            builder.Services.AddTransient<StatisticsPageViewModel>();
            builder.Services.AddTransient<PlayerSelectorViewModel>();
            builder.Services.AddTransient<CreateNewPlayerViewModel>();
            builder.Services.AddTransient<OnlineGamePageViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<RegisterPageViewModel>();
            builder.Services.AddTransient<LobbySelectorPageViewModel>();
            builder.Services.AddTransient<OnlineSettingsViewModel>();
            builder.Services.AddTransient<LobbyRoomPageViewModel>(); 
            builder.Services.AddTransient<MatchSummaryPageViewModel>();
            builder.Services.AddTransient<CheckoutPageViewModel>();
            builder.Services.AddTransient<DoubleTriesPageViewModel>();

            builder.Services.AddSingleton<GameService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<LocalPlayerDbService>();
            builder.Services.AddSingleton<LocalGameDbService>();
            builder.Services.AddSingleton<PasswordService>();
            builder.Services.AddSingleton<StatisticsService>();

            builder.Services.AddHttpClient<IRestPlayerDataService, RestPlayerDataService>();
            builder.Services.AddHttpClient<IRestLobbyDataService, RestLobbyDataService>();
            builder.Services.AddSingleton<ISignalRLobbyService, SignalRLobbyService>(); 
            builder.Services.AddSingleton<ISignalRGameService, SignalRGameService>();
            builder.Services.AddSingleton<IRestGameDataService, RestGameDataService>();
            builder.Services.AddSingleton<IOnlineGameService, OnlineGameService>();


            return builder.Build();
        }
    }
}
