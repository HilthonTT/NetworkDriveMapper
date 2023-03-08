namespace NetworkDriveMapper.Helpers;

public class ConnectorHelper
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
    
    public async Task ConnectDrivesAsync(ObservableCollection<DriveModel> driveList, 
                                         ObservableCollection<DriveModel> connectedDrives)
    {
        var settings = await _appSettingsService.GetSettings();

        _settings.AutoConnectOnStartUp = settings is not null ? settings.AutoConnectOnStartUp : true;

        var drives = await _driveService.GetDriveList();

        if (driveList.Count != 0)
            driveList.Clear();

        foreach (var drive in drives)
        {
            drive.IsConnected = false;
            drive.ButtonColor = Green;
            driveList.Add(drive);
        }

        if (_settings.AutoConnectOnStartUp)
        {
            foreach (var drive in driveList)
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
                        connectedDrives.Add(drive);
                        drive.ButtonColor = Green;
                    }
                    else
                    {
                        drive.ButtonColor = Red;
                    }
                }
            }
        }
    }

    public async Task DisconnectDrivesAsync(ObservableCollection<DriveModel> drives,
                                            ObservableCollection<DriveModel> connectedDrives)
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
                                                ObservableCollection<DriveModel> connectedDrives)
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
                connectedDrives.Add(drive);
            }
            else
            {
                drive.ButtonColor = Red;
            }
        }
    }

    public async Task DisconnectSingularDriveAsync(DriveModel drive,
                                                   ObservableCollection<DriveModel> connectedDrives)
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
        }
    }



}
