namespace NetworkDriveMapper.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IAppSettingsService _appSettingsService;

    public SettingsViewModel(IAppSettingsService appSettingsService)
    {
        Title = "Network Drive Mapper";
        _appSettingsService = appSettingsService;
    }

    [ObservableProperty]
    private AppSettings _settings;

    /// <summary>
    /// Fetchs the AppSettings that is in database.
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task GetSettings()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var settings = await _appSettingsService.GetSettings();

            if (settings is null)
            {
                var appSettings = new AppSettings
                {
                    AutoConnectOnStartUp = true
                };

                await _appSettingsService.InsertSettings(appSettings);
                settings = await _appSettingsService.GetSettings();
                Settings = settings;
            }
            else
            {
                Settings = settings;
            }

            if (Settings.AutoConnectOnStartUp is true)
            {
                Settings.AutoConnectButtonColor = Green;
            }
            else
            {
                Settings.AutoConnectButtonColor = Red;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get settings: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ChangeValueAutoConnect()
    {
        Settings.AutoConnectOnStartUp = !Settings.AutoConnectOnStartUp;
        if (Settings.AutoConnectOnStartUp is true)
        {
            Settings.AutoConnectButtonColor = Green;
        }
        else
        {
            Settings.AutoConnectButtonColor = Red;
        }
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        try
        {
            var response = await _appSettingsService.SaveSettings(Settings);

            if (response > 0)
            {
                await Shell.Current.DisplayAlert("Success!",
                    $"The Settings were saved!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to save settings", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to save settings: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}", true);
    }

    [RelayCommand]
    private async Task GoToAddDriveAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(AddDrivePage)}", true);
    }

    [RelayCommand]
    private async Task GoToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}
