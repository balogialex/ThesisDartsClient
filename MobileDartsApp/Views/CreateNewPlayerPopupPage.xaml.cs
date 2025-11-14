using MobileDartsApp.ViewModels;
namespace MobileDartsApp.Views;

public partial class CreateNewPlayerPopupPage : Mopups.Pages.PopupPage
{
	private CreateNewPlayerViewModel _viewModel;
	public CreateNewPlayerPopupPage(CreateNewPlayerViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.StatusMessage = string.Empty;
		_viewModel.PictureSource = "profile.png";
		_viewModel.PlayerName = string.Empty;
    }
}