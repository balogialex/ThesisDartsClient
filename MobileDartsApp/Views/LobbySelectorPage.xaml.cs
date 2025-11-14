using CommunityToolkit.Mvvm.Input;
using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class LobbySelectorPage : ContentPage
{
	private LobbySelectorPageViewModel _viewModel;
	public LobbySelectorPage(LobbySelectorPageViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		this.BindingContext = _viewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LobbySelectorPageViewModel vm)
            await vm.OnAppearingAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is LobbySelectorPageViewModel vm)
            await vm.OnDisappearingAsync();
    }
}