namespace NetworkDriveMapper.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    public const string Red = "#FF0000";
    public const string Green = "#00FF00";
    public const string ErrorMessage = "Your platform is unsupported";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private ObservableCollection<DriveModel> _drives = new();

    [ObservableProperty]
    private List<DriveModel> _connectedDrives = new(); // Counts how many drives are connected
}
