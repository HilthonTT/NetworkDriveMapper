namespace NetworkDriveMapper.ViewModels;

public partial class AddViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly DrivesViewModel _drivesViewModel;

    public AddViewModel(IDriveService driveService, 
                        DrivesViewModel drivesViewModel)
    {
        Title = "Add a drive";
        _driveService = driveService;
        _drivesViewModel = drivesViewModel;
    }

    [ObservableProperty]
    private string _letter;

    [ObservableProperty]
    private string _address;

    [ObservableProperty]
    private string _driveName;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _userName;


    [RelayCommand]
    private async Task AddDriveAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Letter) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(DriveName) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(UserName))
            {
                await Shell.Current.DisplayAlert("Error!",
                    "Unable to add drive: every field must be populated.", "OK");

                return;
            }

            var driveModel = new DriveModel
            {
                Letter = Letter,
                Address = Address,
                DriveName = DriveName,
                Password = Password,
                UserName = UserName
            };

            var response = await _driveService.AddDrive(driveModel);

            _drivesViewModel.Drives.Add(driveModel);

            if (response > 0)
                await Shell.Current.DisplayAlert("Drive Added!",
                    $"The drive {DriveName} hasb enn added", "OK", "Cancel").ContinueWith(async (result) =>
                    {
                        if (result.Result)
                        {
                            await Shell.Current.Navigation.PopToRootAsync(true);
                        }
                        else
                        {

                        }
                    });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to add drive: {ex.Message}", "OK");
        }
    }
}
