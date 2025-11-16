using MobileDartsApp.ViewModels;
using Mopups.Pages;
using Mopups.Services;
using System.Drawing;

namespace MobileDartsApp;

public partial class CheckoutPopupPage : PopupPage
{
    public CheckoutPageViewModel ViewModel;
    public  CheckoutPopupPage(CheckoutPageViewModel viewModel)
    {
        InitializeComponent();ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}