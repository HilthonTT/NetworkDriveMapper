namespace NetworkDriveMapper.Helpers;

public interface IConnectorHelper
{
    Task ConnectDriveAsync(DriveModel drive, List<DriveModel> connectedDrives);
    Task DisconnectDriveAsync(DriveModel drive, List<DriveModel> connectedDrives);
}