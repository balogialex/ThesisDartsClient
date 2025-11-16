using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Entities;
using MobileDartsApp.Models;
using MobileDartsApp.Services;
using MobileDartsApp.Views;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public partial class PlayerSelectorViewModel : ObservableObject
    {
        #region fields
        private readonly LocalPlayerDbService playerDbService;
        private readonly SettingsService settingsService;
        private readonly CreateNewPlayerPopupPage createPlayerPage;

        private string _filterText;
        private ObservableCollection<object> selectedPlayers = new();
        #endregion

        #region Properties
        public ObservableCollection<PlayerModel> FilteredPlayers { get; } = new();
        public ObservableCollection<object> SelectedPlayers
        {
            get => selectedPlayers;
            set
            {
                if (selectedPlayers != value)
                {
                    selectedPlayers = value;
                    OnPropertyChanged(nameof(SelectedPlayers));
                }
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    _ = InitializePlayersAsync();
                    OnPropertyChanged(nameof(FilterText));
                }
            }
        }
        #endregion

        #region Commands
        public IAsyncRelayCommand ContinueExecute { get; }
        public IAsyncRelayCommand CreatePlayerExecute { get; }
        public IAsyncRelayCommand<PlayerModel> DeletePlayerExecute { get; }
        #endregion

        public PlayerSelectorViewModel(
            SettingsService settingsService,
            LocalPlayerDbService playerDbService,
            CreateNewPlayerPopupPage createPlayerPage)
        {
            this.playerDbService = playerDbService;
            this.settingsService = settingsService;
            this.createPlayerPage = createPlayerPage;

            ContinueExecute = new AsyncRelayCommand(OnContinuePressedAsync);
            CreatePlayerExecute = new AsyncRelayCommand(OnCreatePlayerPressedAsync);
            DeletePlayerExecute = new AsyncRelayCommand<PlayerModel>(OnDeletePlayerPressedAsync);

            _ = InitializePlayersAsync();
        }

        private async Task OnCreatePlayerPressedAsync()
        {
            await MopupService.Instance.PushAsync(createPlayerPage);

            while (MopupService.Instance.PopupStack.Count > 0)
                await Task.Delay(100);

            await InitializePlayersAsync();
        }

        private async Task OnContinuePressedAsync()
        {
            var picked = SelectedPlayers.OfType<PlayerModel>().ToList();
            if (picked.Count == 0) return;

            if (settingsService.CurrentSettings is not LocalGameSettings local)
            {
                local = new LocalGameSettings();
                settingsService.LoadLocalSettings(local);
            }

            local.OrderOfPlayers = picked.Select(p => p.Name).ToList();
            local.Players = picked;

            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private async Task InitializePlayersAsync()
        {
            SelectedPlayers.Clear();

            var players = await playerDbService.GetAllPlayerEntities();

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                foreach (var p in players)
                {
                    if (!FilteredPlayers.Any(fp => fp.Name == p.Name))
                        FilteredPlayers.Add(new PlayerModel { Name = p.Name, PictureSource = p.PictureSource });
                }
            }
            else
            {
                var filtered = players
                    .Where(p => p.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredPlayers.Clear();
                foreach (var p in filtered)
                    FilteredPlayers.Add(new PlayerModel { Name = p.Name, PictureSource = p.PictureSource });
            }
        }

        private async Task OnDeletePlayerPressedAsync(PlayerModel player)
        {
            if (player is null) return;

            FilteredPlayers.Remove(player);
            var entity = await playerDbService.GetByName(player.Name);
            if (entity != null)
                await playerDbService.Delete(entity);

            await InitializePlayersAsync();
        }
    }
}
