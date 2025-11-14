using MobileDartsApp.ViewModels;

namespace MobileDartsApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}