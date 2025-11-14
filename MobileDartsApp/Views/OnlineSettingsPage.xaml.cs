using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class OnlineSettingsPage : ContentPage
{
	private OnlineSettingsViewModel _viewModel;
	public OnlineSettingsPage(OnlineSettingsViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}
}