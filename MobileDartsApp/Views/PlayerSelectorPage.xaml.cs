using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class PlayerSelectorPage : ContentPage
{
	PlayerSelectorViewModel _viewModel;
	public PlayerSelectorPage(PlayerSelectorViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}
}