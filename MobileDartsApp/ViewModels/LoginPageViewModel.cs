using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.DataServices.Rest;
using MobileDartsApp.Services.Online.Dtos.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MobileDartsApp.ViewModels
{
    public class LoginPageViewModel : ObservableObject
    {
        #region Fields
        private PasswordService _passwordService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _statusMessage = string.Empty;
        private IRestPlayerDataService _restPlayerService;
        private bool _isBusy;
        #endregion
        #region Properties
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        public bool IsBusy 
        { 
            get => _isBusy; 
            set => SetProperty(ref _isBusy, value); }
        #endregion
        #region Commands
        public IAsyncRelayCommand LoginCommand { get; }
        #endregion
        public LoginPageViewModel(PasswordService passwordService, IRestPlayerDataService restPlayerDataService)
        {
            _passwordService = passwordService;
            _restPlayerService = restPlayerDataService;
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Username) &&
                    !string.IsNullOrWhiteSpace(Password))
                {
                    IsBusy = true;
                    var result = await _restPlayerService.LoginPlayerAsync(
                            new PlayerDto { Username = Username, 
                                            Password = _passwordService.HashPassword(Password) });
                    StatusMessage = result.Item2;
                    if (result.Item1)
                    {
                        var currentUser = await _restPlayerService.GetCurrentUserAsync();
                        if (currentUser != null)
                        {
                            StatusMessage = $"Welcome, {currentUser.Username}!";
                        }
                    }

                }
                else
                {
                    StatusMessage = "Tölts ki minden mezőt rendesen!";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
