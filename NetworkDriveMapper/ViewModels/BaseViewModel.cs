namespace NetworkDriveMapper.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

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
