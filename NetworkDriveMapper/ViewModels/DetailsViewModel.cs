namespace NetworkDriveMapper.ViewModels;

[QueryProperty("Drive", "Drive")]
public partial class DetailsViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    public DetailsViewModel(IDriveService driveService)
    {
        _driveService = driveService;
    }

    [ObservableProperty]
    private DriveModel _drive;


    [RelayCommand]
    private async Task DisplayActionAsync(DriveModel drive)
    {
        var response = await Shell.Current.DisplayActionSheet("Select Option", "OK", null, "Delete", "Cancel");

        if (response == "Delete")
        {
            var deleteReponse = await _driveService.DeleteDrive(drive);

            Drives.Remove(drive);

            if (deleteReponse > 0)
                await Shell.Current.DisplayAlert("Success",
                   $"Drive {drive.DriveName} has been deleted", "OK", "Cancel").ContinueWith(async (result) =>
                   {
                       if (result.Result)
                       {
                           await Shell.Current.Navigation.PopToRootAsync(true);
                       }
                       else
                       {
                           await Shell.Current.Navigation.PopToRootAsync(true);
                       }
                   });
        }
        else
        {

        }
    }

    [RelayCommand]
    private async Task UpdateDriveAsync()
    {
        try
        {
            var response = await _driveService.UpdateDrive(Drive);

            await Shell.Current.DisplayAlert("Drive Updated!",
                $"Drive {Drive.DriveName} has been updated!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to update drive: {ex.Message}", "OK");
        }
    }
}
