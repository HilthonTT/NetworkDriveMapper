using NetworkDriveMapper.Helpers;

namespace NetworkDriveMapper.ViewModels;

public partial class AddViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly IAesEncryptionHelper _encryption;

    public AddViewModel(IDriveService driveService, 
                        IAesEncryptionHelper encryption)
    {
        Title = "Network Drive Mapper";
        _driveService = driveService;
        _encryption = encryption;
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
                    "Unable to add drive: Every field must be populated.", "OK");

                return;
            }

            if (Letter.Length > 1)
            {
                await Shell.Current.DisplayAlert("Error!",
                    "Unable to add drive: The Letter must only contain 1 character.", "OK");

                return;
            }

            var driveModel = new DriveModel
            {
                Letter = await _encryption.EncryptAsync(Letter.ToUpper()),
                Address = await _encryption.EncryptAsync(Address),
                DriveName = await _encryption.EncryptAsync(DriveName),
                Password = await _encryption.EncryptAsync(Password),
                UserName = await _encryption.EncryptAsync(UserName),
            };

            var response = await _driveService.AddDrive(driveModel);

            var driveList = await _driveService.GetDriveList();

            Drives.Clear();
            foreach (var drive in driveList)
            {   
                Drives.Add(drive);
            }

            if (response > 0)
                await Shell.Current.DisplayAlert("Drive Added!",
                    $"The drive {DriveName} has been added!", "OK", "Cancel").ContinueWith(async (result) =>
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
