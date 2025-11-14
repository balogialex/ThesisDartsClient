using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.Services;
using MobileDartsApp.Services.Online.Dtos.Player;
using MobileDartsApp.Services.Online.DataServices.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.ViewModels
{
    public class RegisterPageViewModel : ObservableObject
    {
        #region Fields
        private PasswordService _passwordService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isBusy;
        private IRestPlayerDataService _restPlayerService;
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
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        #endregion
        #region commands
        public IAsyncRelayCommand RegisterCommand { get; }
        #endregion
        public RegisterPageViewModel(PasswordService passwordService, IRestPlayerDataService restPlayerService)
        {
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            _passwordService = passwordService;
            _restPlayerService = restPlayerService;
        }
        private async Task RegisterAsync()
        {
            if (!string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                Password == ConfirmPassword)
            {
                try
                {
                    IsBusy = true;
                    var result = await _restPlayerService.RegisterPlayerAsync(
                            new PlayerDto { Username = Username, 
                                            Password = _passwordService.HashPassword(Password) });
                    StatusMessage = result.Item2;
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
            else
            {
                StatusMessage = "Tölts ki minden mezőt helyesen!";
            }
            return;
        }
    }
}
