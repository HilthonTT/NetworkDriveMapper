namespace NetworkDriveMapper.Models;

public interface ILoggedInAppSettings
{
    bool AutoConnectOnStartUp { get; set; }
    bool LaunchOnStartUp { get; set; }
}