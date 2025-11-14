using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Entities;
using MobileDartsApp.Models;
using MobileDartsApp.Services;
using MobileDartsApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class SettingsPageViewModel : ObservableObject
    {
        #region fields
        private int _selectedNumberOfLegs = 1;
        private LegFormat _selectedLegFormat;
        private int _selectedGameMode;
        #endregion

        #region properties
        public ObservableCollection<int> NumberOfLegs { get; }
        public List<int> Gamemodes { get; }
        public List<LegFormat> LegFormats { get; }
        public int SelectedNumberOfLegs
        {
            get => _selectedNumberOfLegs;
            set
            {
                _selectedNumberOfLegs = value;
                OnPropertyChanged(nameof(SelectedNumberOfLegs));
            }
        }
        public int SelectedGameMode
        {
            get => _selectedGameMode;
            set
            {
                _selectedGameMode = value;
                OnPropertyChanged(nameof(SelectedGameMode));
            }
        }
        public LegFormat SelectedLegFormat
        {
            get => _selectedLegFormat;
            set
            {
                _selectedLegFormat = value;
                OnPropertyChanged(nameof(SelectedLegFormat));
            }
        }
        #endregion

        #region commands
        public ICommand StartGame { get; }
        public ICommand MinusLeg { get; }
        public ICommand PlusLeg { get; }
        #endregion

        private SettingsService settings;
        
        public SettingsPageViewModel(SettingsService settingsService)
        {
            settings = settingsService;
            NumberOfLegs = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };
            _selectedNumberOfLegs = 3;
            Gamemodes = new List<int> { 301, 501, 701 };
            SelectedGameMode = 501;
            LegFormats = Enum.GetValues(typeof(LegFormat)).Cast<LegFormat>().ToList();
            SelectedLegFormat = LegFormat.FirstTo;

            StartGame = new Command(startGameExecute);
            PlusLeg = new Command(plusLegExecute);
            MinusLeg = new Command(minusLegExecute);
        }
        


        private void minusLegExecute()
        {
            if (SelectedNumberOfLegs - 1 < 1 )
                return;
            else
                SelectedNumberOfLegs -= 1;
        }

        private void plusLegExecute()
        {
            if (SelectedNumberOfLegs + 1 > 15)
                return;
            else
                SelectedNumberOfLegs += 1;
        }

        private async void startGameExecute()
        {
                settings.CurrentSettings.LegCount = _selectedNumberOfLegs;
                settings.CurrentSettings.MatchFormat = MatchFormat.Leg;
                settings.CurrentSettings.LegFormat = _selectedLegFormat;
                settings.CurrentSettings.StartScore = _selectedGameMode;

                await Shell.Current.GoToAsync(nameof(GamePage));
        }
    }
}
