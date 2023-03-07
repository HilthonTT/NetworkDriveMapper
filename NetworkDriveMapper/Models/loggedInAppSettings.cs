namespace NetworkDriveMapper.Models;

public class loggedInAppSettings : IloggedInAppSettings
{
    public bool AutoConnectOnStartUp { get; set; }
    public bool AutoMinimizeAfterConnect { get; set; }
}
