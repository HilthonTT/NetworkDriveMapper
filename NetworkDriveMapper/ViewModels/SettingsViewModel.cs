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
                    AutoConnectOnStartUp = true,
                    AutoMinimizeAfterConnect = false,
                };

                await _appSettingsService.InsertSettings(appSettings);
                settings = await _appSettingsService.GetSettings();
                Settings = settings;
            }
            else
            {
                Settings = settings;
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
}
