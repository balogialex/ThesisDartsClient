using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class ProfilePage : ContentPage
{
    private ProfilePageViewModel _viewModel;

    public ProfilePage(ProfilePageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.UpdateProfileAsync();
    }
}