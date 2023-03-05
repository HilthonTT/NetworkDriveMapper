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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.IsConnected == false)
            _viewModel.GetDrivesCommand.Execute(null);
    }
}