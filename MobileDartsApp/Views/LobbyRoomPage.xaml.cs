using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class LobbyRoomPage : ContentPage
{
	private LobbyRoomPageViewModel _viewModel;
	public LobbyRoomPage(LobbyRoomPageViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}
    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.LeaveLobbyCommand.CanExecute(null))
        {
            _viewModel.LeaveLobbyCommand.Execute(null);
            return true;
        }
        return true;
    }
}