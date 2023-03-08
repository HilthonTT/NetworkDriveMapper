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

    public Color AutoConnectColorAsColor => Color.FromArgb(AutoConnectButtonColor);
}
