using MobileDartsApp.Models;
using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class GamePage : ContentPage
{
	private GamePageViewModel _viewModel;
    public GamePage(GamePageViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
        BindingContext = _viewModel;
	}
    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.ForfeitCommand.CanExecute(null))
        {
            _viewModel.ForfeitCommand.Execute(null);
            return true;
        }
        return base.OnBackButtonPressed();
    }
}