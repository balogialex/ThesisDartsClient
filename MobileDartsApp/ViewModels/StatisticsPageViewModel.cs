using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Entities;
using MobileDartsApp.Models;
using MobileDartsApp.Services;
using System.Collections.ObjectModel;

namespace MobileDartsApp.ViewModels
{
    public class StatisticsPageViewModel : ObservableObject
    {
        private StatisticsModel _legStats;
        public StatisticsModel LegStats
        {
            get => _legStats;
            set
            {
                _legStats = value;
                OnPropertyChanged(nameof(LegStats));
            }
        }
    
        public StatisticsPageViewModel(StatisticsService statService, LocalGameDbService gameDbService)
        {
            _statisticsService = statService;
            _gameDbService = gameDbService;
            SelectedLeg = null;
            LoadGameStats();
            LoadLegs();
        }

        private LocalGameDbService _gameDbService;
        private StatisticsService _statisticsService;
        #region Properties

        private StatisticsModel _gameStats;
        public StatisticsModel GameStats
        {
            get => _gameStats;
            set
            {
                _gameStats = value;
                OnPropertyChanged(nameof(GameStats));
            }
        }

        public ObservableCollection<LegEntity> Legs { get; set; } = new ObservableCollection<LegEntity>();
        public LegEntity? SelectedLeg
        {
            get => _statisticsService.CurrentLeg;
            set
            {
                _statisticsService.CurrentLeg = value;
                LoadLegStats();
                OnPropertyChanged(nameof(SelectedLeg));
            }
        }
        #endregion
        private async void LoadGameStats()
        {
            GameStats = await _statisticsService.GetStatsForGame();
        }
        private async void LoadLegs()
        {
            Legs.Clear();
            foreach (LegEntity leg in await _gameDbService.GetAllLegsInGame(_statisticsService.CurrentGame.Id))
            {
                Legs.Add(leg);
            }
        }
        private async void LoadLegStats()
        {
            if (SelectedLeg!=null)
            {
                LegStats = await _statisticsService.GetStatsForLeg();
            }
        }

    }
}
