using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class StatisticsPage : ContentPage
{
	private StatisticsPageViewModel _viewModel;
    public StatisticsPage(StatisticsPageViewModel vm)
	{
		InitializeComponent();
        _viewModel = vm;
        BindingContext = _viewModel;
	}
}