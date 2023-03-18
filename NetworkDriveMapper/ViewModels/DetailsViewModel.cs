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

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}", true);
    }

    [RelayCommand]
    private async Task GoToAddDriveAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(AddDrivePage)}", true);
    }

    [RelayCommand]
    private async Task GoToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}
