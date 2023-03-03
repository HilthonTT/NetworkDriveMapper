using NetworkDriveMapper.Services;

namespace NetworkDriveMapper.ViewModel;

public partial class DetailsViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    public DetailsViewModel(IDriveService driveService)
    {
        _driveService = driveService;
    } 

    [RelayCommand]
    private async Task DisplayActionAsync(DriveModel drive)
    {
        var response = await Shell.Current.DisplayActionSheet("Select Option", "OK", null, "Update", "Delete");

        if (response == "Update")
        {
            await Shell.Current.GoToAsync(nameof(DetailsPage), true,
                new Dictionary<string, object>
                {
                     { "Drive", drive }
                });
        }
        else if (response == "Delete")
        {
            var deleteReponse = await _driveService.DeleteDrive(drive);

            if (deleteReponse > 0)
            {
                await Shell.Current.DisplayAlert("Success",
                    $"Drive {drive.DriveName} has been deleted", "OK");
            }
        }
    }
}
