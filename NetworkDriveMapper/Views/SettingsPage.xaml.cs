namespace NetworkDriveMapper.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _settingsViewModel;

    public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		BindingContext = settingsViewModel;
        _settingsViewModel = settingsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
		if (_settingsViewModel.Settings is null)
		{
            _settingsViewModel.GetSettingsCommand.Execute(null);
		}
    }
}