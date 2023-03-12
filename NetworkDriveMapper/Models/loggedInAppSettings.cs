namespace NetworkDriveMapper.Models;

public class LoggedInAppSettings : ILoggedInAppSettings
{
    public bool AutoConnectOnStartUp { get; set; }
    public bool LaunchOnStartUp { get; set; } = false;
}
