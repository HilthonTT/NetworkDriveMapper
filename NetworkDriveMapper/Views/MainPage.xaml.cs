namespace NetworkDriveMapper;

public partial class MainPage : ContentPage
{
    private readonly DrivesViewModel _viewModel;
    private readonly IDriveService _driveService;

    public MainPage(DrivesViewModel viewModel, IDriveService driveService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _driveService = driveService;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var driveList = await _driveService.GetDriveList();

        if (_viewModel.IsConnected == false)
            _viewModel.GetDrivesCommand.Execute(null);

        if (_viewModel.Drives.Count != driveList.Count)
        {
            foreach (var drive in driveList)
            {
                if (_viewModel.Drives.Contains(drive) == false)
                {
                    _viewModel.Drives.Add(drive);
                }
            }
        }
    }
}