using NetworkDriveMapper.Helpers;

namespace NetworkDriveMapper.ViewModels;

[QueryProperty("Drive", "Drive")]
public partial class DetailsViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly IAesEncryptionHelper _encryption;

    public DetailsViewModel(IDriveService driveService,
                            IAesEncryptionHelper encryption)
    {
        _driveService = driveService;
        _encryption = encryption;
        Title = "Network Drive Mapper";
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
            if (string.IsNullOrWhiteSpace(Drive.Letter) ||
                string.IsNullOrWhiteSpace(Drive.Address) ||
                string.IsNullOrWhiteSpace(Drive.DriveName) ||
                string.IsNullOrWhiteSpace(Drive.Password) ||
                string.IsNullOrWhiteSpace(Drive.UserName))
            {
                await Shell.Current.DisplayAlert("Error!",
                    "Unable to add drive: Every field must be populated.", "OK");

                return;
            }

            if (Drive.Letter.Length > 1)
            {
                await Shell.Current.DisplayAlert("Error!",
                    "Unable to add drive: The Letter must only contain 1 character.", "OK");

                return;
            }

            var drive = new DriveModel()
            {
                Id = Drive.Id,
                Letter = await _encryption.EncryptAsync(Drive.Letter),
                Address = await _encryption.EncryptAsync(Drive.Address),
                DriveName = await _encryption.EncryptAsync(Drive.DriveName),
                Password = await _encryption.EncryptAsync(Drive.Password),
                UserName = await _encryption.EncryptAsync(Drive.UserName),
            };

            var response = await _driveService.UpdateDrive(drive);

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
