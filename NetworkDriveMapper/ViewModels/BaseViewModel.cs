namespace NetworkDriveMapper.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetDriveListCommand))]
    private ObservableCollection<DriveModel> _drives = new();

    [ObservableProperty]
    private List<DriveModel> _connectedDrives = new(); // Counts how many drives are connected

    [RelayCommand]
    private async Task SetDriveList()
    {
        var driveService = new DriveService();
        var driveList = await driveService.GetDriveList();
        Drives = new ObservableCollection<DriveModel>(driveList);
    }
}
