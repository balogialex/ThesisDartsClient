using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class RegisterPage : ContentPage
{
	private RegisterPageViewModel _viewModel;
	public RegisterPage(RegisterPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}