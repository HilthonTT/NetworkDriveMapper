namespace NetworkDriveMapper.Services
{
    public interface IDriveMapperService
    {
        Task ChecksForConnectedDrivesAsync(DriveModel drive);
        Task ChecksForConnectedDrivesMacOSAsync(DriveModel drive);
        Task ConnectDriveAsync(DriveModel drive);
        Task ConnectDriveMacOSAsync(DriveModel drive);
        Task DisconnectDrivesAsync(DriveModel drive);
        Task DisconnectDrivesMacOSAsync(DriveModel drive);
        bool IsError();
    }
}