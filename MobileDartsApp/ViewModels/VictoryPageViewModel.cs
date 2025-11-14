using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class VictoryPageViewModel: ObservableObject
    {
        
        private GameService gameService;
        private LocalGameDbService localGameDbService;
        private string _feedBackMessage;
        public string FeedBackMessage 
        { 
            get => _feedBackMessage;
            set
            {
                _feedBackMessage = value;
                OnPropertyChanged(nameof(FeedBackMessage));
            } 
        }
        private bool _canSave;
        public bool CanSave {
            get=>_canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged(nameof(CanSave));
            }
        }
        public ICommand SaveStatisticsCommand { get; set; }
        public IAsyncRelayCommand NavigateToRootCommand { get; set; }
        public VictoryPageViewModel(GameService gameService, LocalGameDbService localGameDbService)
        {
            this.gameService = gameService;
            this.localGameDbService = localGameDbService;
            SaveStatisticsCommand = new Command(savestats);
            NavigateToRootCommand = new AsyncRelayCommand(navigateToRootExecute);
            FeedBackMessage = "Nyomd meg a gombot a statisztikák mentése érdekében.";
            CanSave = true;
        }

        private async Task navigateToRootExecute()
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }


        private async void savestats()
        {
            try
            {
                CanSave = false;
                FeedBackMessage = "Mentés folyamatban";
                await localGameDbService.Create(gameService.CurrentGame);
                FeedBackMessage = "Sikeresen mentette a statisztikákat.";
            }
            catch (Exception ex)
            {
                CanSave = true;
                FeedBackMessage = ex.Message;
            }
        }
    }
}
