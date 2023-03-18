namespace NetworkDriveMapper.Helpers;

public class ConnectorHelper : IConnectorHelper
{
    private readonly IDriveMapperService _driveMapperService;

    private const string Red = "#FF0000";
    private const string Green = "#00FF00";
    private const string ErrorMessage = "Your platform is unsupported.";

    public ConnectorHelper(IDriveMapperService driveMapperService)
    {
        _driveMapperService = driveMapperService;
    }

    public async Task ChecksForConnectedDrivesAsync(DriveModel drive, List<DriveModel> connectedDrives)
    {
        if (OperatingSystem.IsWindows())
             await _driveMapperService.ChecksForConnectedDrivesAsync(drive);
        else if (OperatingSystem.IsMacOS())
            await _driveMapperService.ChecksForConnectedDrivesMacOSAsync(drive);
        else
            await Shell.Current.DisplayAlert("Error!",
                ErrorMessage, "OK");

        if (_driveMapperService.IsError() == false)
        {
            SetPropertyToConnected(drive);
            connectedDrives.Add(drive);
        }
        else
        {
            SetPropertyToDisconnected(drive);
        }
    }

    public async Task ConnectDriveAsync(DriveModel drive, List<DriveModel> connectedDrives)
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
                SetPropertyToConnected(drive);
                connectedDrives.Add(drive);
            }
            else
            {
                SetPropertyToDisconnected(drive);
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Warning!",
                $"Drive {drive.Letter} is already mounted.", "OK");
        }
    }

    public async Task DisconnectDriveAsync(DriveModel drive, List<DriveModel> connectedDrives)
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
            SetPropertyToDisconnected(drive);
        }
        else
        {
            await Shell.Current.DisplayAlert("Warning!",
                $"Drive {drive.Letter} is already unmounted.", "OK");
        }
    }

    private static void SetPropertyToConnected(DriveModel drive)
    {
        drive.ButtonColor = Green;
        drive.IsConnected = true;
    }

    private static void SetPropertyToDisconnected(DriveModel drive)
    {
        drive.ButtonColor = Red;
        drive.IsConnected = false;
    }
}
