namespace NetworkDriveMapper.Services
{
    public interface INetUseService
    {
        Task ConnectDriveAsync(DriveModel drive);
        Task ConnectDriveMacOSAsync(DriveModel drive);
    }
}