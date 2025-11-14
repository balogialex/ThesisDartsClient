using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class VictoryPage : ContentPage
{
    private VictoryPageViewModel _viewModel;
	public VictoryPage(VictoryPageViewModel vm)
	{
		InitializeComponent();
        _viewModel = vm;
		BindingContext = _viewModel;
	}
    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.NavigateToRootCommand.CanExecute(null))
        {
            _viewModel.NavigateToRootCommand.Execute(null);
            return true;
        }
        return base.OnBackButtonPressed();
    }
}