using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class MatchSummaryPage : ContentPage
{
    private readonly MatchSummaryPageViewModel _viewModel;
    public MatchSummaryPage(MatchSummaryPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
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