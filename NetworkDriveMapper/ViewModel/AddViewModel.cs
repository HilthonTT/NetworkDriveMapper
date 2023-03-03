using NetworkDriveMapper.Services;

namespace NetworkDriveMapper.ViewModel;

public partial class AddViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;

    public AddViewModel(IDriveService driveService)
    {
        Title = "Add a drive";
        _driveService = driveService;
    }

    [ObservableProperty]
    private readonly DriveModel _drive;

    [RelayCommand]
    private async Task AddDriveAsync()
    {
        try
        {
            var response = await _driveService.AddDrive(new DriveModel
            {
                Letter = Drive.Letter,
                Address = Drive.Address,
                DriveName = Drive.DriveName,
                Password = Drive.Password,
                UserName = Drive.UserName
            });

            if (response > 0)
                await Shell.Current.DisplayAlert("Drive Added!",
                    $"The drive {Drive.DriveName} hasb enn added", "OK", "").ContinueWith(async (result) =>
                    {
                        if (result.Result)
                        {
                            await Shell.Current.GoToAsync($"{nameof(MainPage)}", true);
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
