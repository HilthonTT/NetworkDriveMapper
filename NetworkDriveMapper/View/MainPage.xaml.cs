namespace NetworkDriveMapper;

public partial class MainPage : ContentPage
{
    private readonly DrivesViewModel _viewModel;

    public MainPage(DrivesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetDrivesCommand.Execute(this);
    }
}