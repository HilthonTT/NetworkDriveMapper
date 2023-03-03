namespace NetworkDriveMapper.ViewModels;

public partial class DrivesViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;

    public ObservableCollection<DriveModel> Drives { get; } = new();
    public DrivesViewModel(IDriveService driveService)
    {
        Title = "Network Drive Mapper";
        _driveService = driveService;
    }

    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// The method gets all the drives in the Table and then tries to map them all using the Process.
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task GetDrivesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var drives = await _driveService.GetDriveList();

            if (Drives.Count != 0)
                Drives.Clear();

            foreach (var drive in drives)
                Drives.Add(drive);

            if (OperatingSystem.IsWindows())
            {
                foreach (var drive in drives)
                {
                    Process.Start($"/C net use {drive.Letter}: \"\\\\{drive.Address}\\{drive.DriveName}\" {drive.Password} /user:{drive.UserName} /persistent:no");
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                foreach (var drive in drives)
                {
                    Process.Start("sudo", $"-t smbfs //{drive.UserName}:{drive.Password}@{drive.Address}/share {drive.DriveName}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get drives: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToDetailsAsync(DriveModel drive)
    {
        if (drive is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
            new Dictionary<string, object>
            {
                { "Drive", drive }
            });
    }

    /// <summary>
    /// Maps all the drives. Checks if the user's OS is windows or is MacOS since their mapping commands are different.
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ConnectAllDrivesAsync()
    {
        try
        {
            if (Drives?.Count > 0)
                if (OperatingSystem.IsWindows())
                    foreach (var drive in Drives)
                        Process.Start($"/C net use {drive.Letter}: \"\\\\{drive.Address}\\{drive.DriveName}\" {drive.Password} /user:{drive.UserName} /persistent:no");
                else if (OperatingSystem.IsMacOS())
                    foreach (var drive in Drives)
                        Process.Start("sudo", $"-t smbfs //{drive.UserName}:{drive.Password}@{drive.Address}/share {drive.DriveName}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to map drives: {ex.Message}", "OK");
        }
    }
}
