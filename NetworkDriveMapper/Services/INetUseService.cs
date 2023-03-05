namespace NetworkDriveMapper.Services
{
    public interface INetUseService
    {
        Task ConnectDriveAsync(DriveModel drive);
        Task ConnectDriveMacOSAsync(DriveModel drive);
        Task DisconnectDrivesAsync(DriveModel drive);
        Task DisconnectDrivesMacOSAsync(DriveModel drive);
        bool IsError();
    }
}