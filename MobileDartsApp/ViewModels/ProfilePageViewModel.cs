using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Views;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class ProfilePageViewModel : ObservableObject
    {
        #region fields
        private readonly IRestPlayerDataService _restPlayerService;
        private string _username = "Guest";
        private bool _isNotLoggedIn = true;
        #endregion
        #region properties
        public string Username 
        { 
            get =>_username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            } 
        }
        public bool IsNotLoggedIn
        {
            get => _isNotLoggedIn;
            set {
                SetProperty(ref _isNotLoggedIn, value);
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
        public bool IsLoggedIn => !_isNotLoggedIn;
        #endregion
        #region commands
        public IAsyncRelayCommand NavigateToLoginCommand { get; }
        public IAsyncRelayCommand NavigateToRegisterCommand { get; }
        public IAsyncRelayCommand LogoutCommand { get; }
        #endregion
        public ProfilePageViewModel(IRestPlayerDataService restPlayerService)
        {
            _restPlayerService = restPlayerService; 
            NavigateToLoginCommand = new AsyncRelayCommand(async () => 
                        await Shell.Current.GoToAsync(nameof(LoginPage)));

            NavigateToRegisterCommand = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);

        }

        private async Task LogoutAsync()
        {
            _restPlayerService.Logout();
            Username = "Guest";
            IsNotLoggedIn = true;
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        public async Task UpdateProfileAsync()
        {
            try
            {
                var currentUser = await _restPlayerService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    Username = currentUser.Username;
                    IsNotLoggedIn = false;
                }
                else
                {
                    Username = "Guest";
                    IsNotLoggedIn = true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        
    }
}
