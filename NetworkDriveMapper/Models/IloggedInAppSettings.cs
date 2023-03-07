namespace NetworkDriveMapper.Models
{
    public interface IloggedInAppSettings
    {
        bool AutoConnectOnStartUp { get; set; }
        bool AutoMinimizeAfterConnect { get; set; }
    }
}