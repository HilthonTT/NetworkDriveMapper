namespace NetworkDriveMapper.Helpers;

public class ConnectorHelper : IConnectorHelper
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILoggedInAppSettings _settings;
    private readonly IDriveService _driveService;
    private readonly IDriveMapperService _driveMapperService;

    private const string Red = "#FF0000";
    private const string Green = "#00FF00";
    private const string ErrorMessage = "Your platform is unsupported";

    public ConnectorHelper(IAppSettingsService appSettingsService,
                           ILoggedInAppSettings settings,
                           IDriveService driveService,
                           IDriveMapperService driveMapperService)
    {
        _appSettingsService = appSettingsService;
        _settings = settings;
        _driveService = driveService;
        _driveMapperService = driveMapperService;
    }

    public async Task DisconnectDrivesAsync(ObservableCollection<DriveModel> drives,
                                            List<DriveModel> connectedDrives)
    {
        foreach (var drive in drives)
        {
            if (drive.IsConnected)
            {
                if (OperatingSystem.IsWindows())
                    await _driveMapperService.DisconnectDrivesAsync(drive);
                else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    await _driveMapperService.DisconnectDrivesMacOSAsync(drive);
                else
                    await Shell.Current.DisplayAlert("Error!",
                        ErrorMessage, "OK");
                drive.ButtonColor = Red;
                drive.IsConnected = false;
                connectedDrives.Remove(drive);
            }
        }
    }

    public async Task ConnectSingularDriveAsync(DriveModel drive,
                                                List<DriveModel> connectedDrives)
    {
        if (drive.IsConnected is false)
        {
            if (OperatingSystem.IsWindows())
                await _driveMapperService.ConnectDriveAsync(drive);
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                await _driveMapperService.ConnectDriveMacOSAsync(drive);
            else
                await Shell.Current.DisplayAlert("Error!",
                    ErrorMessage, "OK");

            if (_driveMapperService.IsError() == false)
            {
                drive.ButtonColor = Green;
                drive.IsConnected = true;
                connectedDrives.Add(drive);
            }
            else
            {
                drive.ButtonColor = Red;
                drive.IsConnected = false;
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Warning!",
                $"Drive {drive.Letter} is already mounted.", "OK");
        }
    }

    public async Task DisconnectSingularDriveAsync(DriveModel drive,
                                                   List<DriveModel> connectedDrives)
    {
        if (drive.IsConnected)
        {
            if (OperatingSystem.IsWindows())
                await _driveMapperService.DisconnectDrivesAsync(drive);
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                await _driveMapperService.DisconnectDrivesMacOSAsync(drive);
            else
                await Shell.Current.DisplayAlert("Error!",
                    ErrorMessage, "OK");

            connectedDrives.Remove(drive);
            drive.IsConnected = false;
            drive.ButtonColor = Red;
        }
        else
        {
            await Shell.Current.DisplayAlert("Warning!",
                $"Drive {drive.Letter} is already unmounted.", "OK");
        }
    }
}
