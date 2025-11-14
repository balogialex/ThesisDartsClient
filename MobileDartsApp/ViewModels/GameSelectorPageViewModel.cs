using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Entities;
using MobileDartsApp.Services;
using MobileDartsApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class GameSelectorPageViewModel : ObservableObject
    {
        #region Fields
        private LocalGameDbService _gameDbService;
        private LocalPlayerDbService _playerDbService;
        private StatisticsService _statService;
        private PlayerEntity? _selectedPlayer;
        private bool _isNotRefreshingData;
        private DateTime _from;
        private DateTime _until;

        #endregion
        #region Properties
        public ObservableCollection<GameEntity> Games { get; set; } = new ObservableCollection<GameEntity>();
        public ObservableCollection<PlayerEntity> Players { get; set; } = new ObservableCollection<PlayerEntity>();
        public PlayerEntity? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                LoadGamesAsync();
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }
        public bool IsNotRefreshingData
        {
            get => _isNotRefreshingData;
            set
            {
                _isNotRefreshingData = value;
                OnPropertyChanged(nameof(IsNotRefreshingData));
            }
        }
        public DateTime From 
        { 
            get=>_from;
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
            }
        }
        public DateTime Until
        {
            get => _until;
            set
            {
                _until = value;
                OnPropertyChanged(nameof(Until));
            }
        }

        #endregion
        #region Commands
        public ICommand RefreshData { get; set; }
        public ICommand LoadPlayers { get; set; }
        public ICommand ElementSelectedCommand { get; }
        public ICommand LoadGamesCommand { get; set; }
        #endregion
        public GameSelectorPageViewModel(LocalGameDbService gameDbService, LocalPlayerDbService playerDbService, StatisticsService statService) 
        {
            _gameDbService = gameDbService;
            _playerDbService = playerDbService;
            _statService = statService;
            From = DateTime.Today;
            Until = DateTime.Today;
            RefreshData = new Command(LoadGamesAsync);
            LoadPlayers = new Command(LoadPlayersAsync);
            ElementSelectedCommand = new Command<GameEntity>(OnElementSelected);
            LoadGamesCommand = new Command(LoadGamesAsync);
        }

        private async void OnElementSelected(GameEntity game)
        {
            _statService.CurrentGame = game;
            _statService.CurrentPlayer = SelectedPlayer;
            await Shell.Current.GoToAsync(nameof(StatisticsPage));
        }

        private async void LoadPlayersAsync()
        {
            IsNotRefreshingData = false;
            List<PlayerEntity> playersList = await _playerDbService.GetAllPlayerEntities();
            foreach (PlayerEntity player in playersList)
            {
                if(!Players.Any(p=>p.Name == player.Name))
                    Players.Add(player);
            }
            IsNotRefreshingData = true;
        }
        private async void LoadGamesAsync()
        {
            IsNotRefreshingData = false;
            Games.Clear();
            if (SelectedPlayer == null)
            {
                return;
            }
            else
            {
                List<GameEntity> gameEntities = await _gameDbService.GetAllGameEntitiesByName(SelectedPlayer);
                foreach (GameEntity gameEntity in gameEntities)
                    Games.Add(gameEntity);
            }
            IsNotRefreshingData = true;
        }
    }
}
