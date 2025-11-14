using MobileDartsApp.ViewModels;
using Mopups.Pages;

namespace MobileDartsApp;

public partial class DoubleTriesPopupPage : PopupPage
{
    public DoubleTriesPageViewModel ViewModel { get; }
    public DoubleTriesPopupPage(DoubleTriesPageViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}