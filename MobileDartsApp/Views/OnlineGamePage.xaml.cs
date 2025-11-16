using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class OnlineGamePage : ContentPage
{
	private OnlineGamePageViewModel _viewModel;
	public OnlineGamePage(OnlineGamePageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		this.BindingContext = _viewModel;
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