namespace NetworkDriveMapper;

public partial class MainPage : ContentPage
{
    private readonly DrivesViewModel _viewModel;

    public MainPage(DrivesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetDrivesCommand.Execute(null);
    }
}