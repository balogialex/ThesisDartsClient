using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class GameSelectorPage : ContentPage
{
	GameSelectorPageViewModel _viewModel;
	public GameSelectorPage(GameSelectorPageViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		this.BindingContext = _viewModel;
	}
    protected override void OnAppearing()
    {
		if (_viewModel.LoadPlayers.CanExecute(null))
		{
			_viewModel.LoadPlayers.Execute(null);
		}

		if (_viewModel.LoadGamesCommand.CanExecute(null))
		{
            _viewModel.LoadGamesCommand.Execute(null);
        }

        base.OnAppearing();
    }
}