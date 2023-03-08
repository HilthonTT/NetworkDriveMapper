namespace NetworkDriveMapper.Helpers;

public interface IConnectorHelper
{
    Task ConnectSingularDriveAsync(DriveModel drive, List<DriveModel> connectedDrives);
    Task DisconnectDrivesAsync(ObservableCollection<DriveModel> drives, List<DriveModel> connectedDrives);
    Task DisconnectSingularDriveAsync(DriveModel drive, List<DriveModel> connectedDrives);
}