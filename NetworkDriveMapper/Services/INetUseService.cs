namespace NetworkDriveMapper.Services
{
    public interface INetUseService
    {
        Task ConnectDriveAsync(string driveLetter, string address, string driveName, string password, string userName);
        Task ConnectDriveMacOSAsync(string address, string driveName, string password, string userName);
    }
}