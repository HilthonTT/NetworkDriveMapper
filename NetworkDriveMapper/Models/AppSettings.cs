namespace NetworkDriveMapper.Models;

public partial class AppSettings : ObservableObject
{
    [ObservableProperty]
    private bool _autoConnectOnStartUp;

    [ObservableProperty]
    private bool _autoMinimizeAfterConnect;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoConnectColorAsColor))]
    private string _autoConnectButtonColor;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoMinimizeColorAsColor))]
    private string _autoMinimizeButtonColor;

    public Color AutoConnectColorAsColor => Color.FromArgb(AutoConnectButtonColor);
    public Color AutoMinimizeColorAsColor => Color.FromArgb(AutoConnectButtonColor);
}
