using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class LoginPage : ContentPage
{
	private LoginPageViewModel _viewModel;
    public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}