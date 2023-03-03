namespace NetworkDriveMapper.ViewModels;

public partial class UpdateViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;

    public UpdateViewModel(IDriveService driveService)
    {
        _driveService = driveService;
    }

    [ObservableProperty]
    private DriveModel _drive;

    [RelayCommand]
    private async Task UpdateDriveAsync()
    {
        try
        {
            var response = await _driveService.UpdateDrive(Drive);

            await Shell.Current.DisplayAlert("Drive Updated!",
                $"Drive {Drive.DriveName} has been updated!", "OK", "").ContinueWith(async (result) =>
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
                $"Unable to update drive: {ex.Message}", "OK");
        }
    }
}
