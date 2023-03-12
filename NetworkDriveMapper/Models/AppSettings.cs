using SQLite;

namespace NetworkDriveMapper.Models;

public partial class AppSettings : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty]
    private bool _autoConnectOnStartUp;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoConnectColorAsColor))]
    private string _autoConnectButtonColor;

    [ObservableProperty]
    private bool _launchOnStartUp = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LaunchColorAsColor))]
    private string _launchButtonColor;
    
    public Color AutoConnectColorAsColor => Color.FromArgb(AutoConnectButtonColor);
    public Color LaunchColorAsColor => Color.FromArgb(LaunchButtonColor);
}
