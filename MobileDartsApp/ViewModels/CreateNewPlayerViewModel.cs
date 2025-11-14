using CommunityToolkit.Mvvm.ComponentModel;
using MobileDartsApp.Entities;
using MobileDartsApp.Services;
using Mopups.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class CreateNewPlayerViewModel : ObservableObject
    {
        #region fields
        private LocalPlayerDbService _playerDbService;
        private string _playerName;
        private string _statusMessage = "Készíts egy új játékost";
        private string _pictureSource = "profile.png";
        #endregion
        #region properties

        public string PictureSource
        {
            get => _pictureSource;
            set
            {
                _pictureSource = value;
                OnPropertyChanged(nameof(PictureSource));
            }
        }
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        #endregion
        public ICommand CreateExecute { get; set; }
        public ICommand PictureSelectExecute { get; set; }

        public CreateNewPlayerViewModel(LocalPlayerDbService localPlayerDbService) 
        {
            CreateExecute = new Command(CreatePlayer);
            PictureSelectExecute = new Command(SelectPicture);
            _playerDbService = localPlayerDbService;
        }

        private async void SelectPicture()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Válaszd ki a játékos profilképét",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
                return;

            string newFilewPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
            using var stream = await result.OpenReadAsync();
            using var newStream = File.Create(newFilewPath);
            await stream.CopyToAsync(newStream);

            PictureSource = newFilewPath;

        }

        private async void CreatePlayer()
        {
            if (!string.IsNullOrEmpty(PlayerName))
            {
                try
                {
                    await _playerDbService.Create(new PlayerEntity()
                    {
                        Name = PlayerName,
                        PictureSource = this.PictureSource,
                    });
                    StatusMessage = $"A felhasználó ({PlayerName}) sikeresen létrehozva";
                    await MopupService.Instance.PopAllAsync();
                }
                catch (SQLiteException)
                {
                    StatusMessage = $"Létezik már ilyen nevű játékos: {PlayerName}.\nVagy valami más probléma adódott.";
                }
            }
            else
            {
                StatusMessage = "Nem megfelelő a játékos neve.";
            }
        }
    }
}
