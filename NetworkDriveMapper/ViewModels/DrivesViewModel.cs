using System.Windows.Input;
using Microsoft.Maui;

namespace NetworkDriveMapper.ViewModels;

public partial class DrivesViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly INetUseService _netUseService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILoggedInAppSettings _settings;

    public DrivesViewModel(IDriveService driveService, 
                            INetUseService netUseService,
                            IAppSettingsService appSettingsService,
                            ILoggedInAppSettings settings)
    {
        Title = "Network Drive Mapper";
        _driveService = driveService;
        _netUseService = netUseService;
        _appSettingsService = appSettingsService;
        _settings = settings;
    }

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private float _driveProgress; // Progress Label, shows the percentage in number

    [ObservableProperty]
    private float _drivePercentage; // The progress bar percentage, changes the progress bar's length.

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

            var settings = await _appSettingsService.GetSettings().WaitAsync(new CancellationToken());
            _settings.AutoConnectOnStartUp = settings.AutoConnectOnStartUp;

            var drives = await _driveService.GetDriveList();

            if (Drives.Count != 0)
                Drives.Clear();

            foreach (var drive in drives)
            {
                drive.IsConnected = false;
                Drives.Add(drive);       
            }

            if (_settings.AutoConnectOnStartUp)
            {
                if (OperatingSystem.IsWindows())
                {
                    foreach (var drive in Drives)
                    {
                        if (drive.IsConnected == false)
                        {
                            await _netUseService.ConnectDriveAsync(drive);
                            if (_netUseService.IsError() == false)
                            {
                                ConnectedDrives.Add(drive);
                                drive.ButtonColor = "#00FF00"; // Green
                            }
                            else
                            {
                                drive.ButtonColor = "#FF0000"; // Red
                            }
                            float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                            DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                            DrivePercentage = DriveProgress / 100;
                        }
                    }
                }
                else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                {
                    foreach (var drive in Drives)
                    {
                        if (drive.IsConnected == false)
                        {
                            await _netUseService.ConnectDriveMacOSAsync(drive);
                            if (_netUseService.IsError() == false)
                            {
                                ConnectedDrives.Add(drive);
                                drive.ButtonColor = "#00FF00"; // Green
                            }
                            else
                            {
                                drive.ButtonColor = "#FF0000"; // Red
                            }
                            float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                            DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                            DrivePercentage = DriveProgress / 100;
                        }
                    }
                }

                IsConnected = true;
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

    /// <summary>
    /// Maps all the drives. 
    /// Checks if the user's OS is windows or is MacOS since their mapping commands are different.
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ConnectAllDrivesAsync()
    {
        try
        {  
            DriveProgress = 0;
            DrivePercentage = 0;
            IsConnected = false;

            foreach (var drive in ConnectedDrives)
            {
                drive.IsConnected = false;
            }

            ConnectedDrives.Clear();

            if (OperatingSystem.IsWindows() && Drives?.Count > 0)
            {
                foreach (var drive in Drives)
                {        
                    if (drive.IsConnected == false)
                    {
                        await _netUseService.ConnectDriveAsync(drive);
                        if (_netUseService.IsError() == false)
                        {
                            ConnectedDrives.Add(drive);
                            drive.ButtonColor = "#00FF00"; // Green
                        }
                        else
                        {
                            drive.ButtonColor = "#FF0000"; // Red
                        }
                        float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                        DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                        DrivePercentage = DriveProgress / 100; 
                    }
                }

                IsConnected = true;
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst() && Drives?.Count > 0)
            {
                foreach (var drive in Drives)
                {
                    if (drive.IsConnected == false)
                    {
                        await _netUseService.ConnectDriveMacOSAsync(drive);
                        if (_netUseService.IsError() == false)
                        {
                            ConnectedDrives.Add(drive);
                            drive.ButtonColor = "#00FF00"; // Green
                        }
                        else
                        {
                            drive.ButtonColor = "#FF0000"; // Red
                        }
                        float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                        DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                        DrivePercentage = DriveProgress / 100;
                        
                    }
                }
                
                IsConnected = true;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to map drives: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Disconnects all mounted drives.
    /// It checks for the OS, do this it's windows and do this if it's MacOS
    /// </summary>
    /// <returns></returns>

    [RelayCommand]
    private async Task DisconnectAllDrivesAsync()
    {
        try
        {
            DriveProgress = 0;
            DrivePercentage = 0;
            IsConnected = false;

            if (OperatingSystem.IsWindows())
            {
                foreach (var drive in Drives)
                {
                    if (drive.IsConnected)
                    {
                        await _netUseService.DisconnectDrivesAsync(drive);
                        drive.ButtonColor = "#FF0000"; // Red
                        drive.IsConnected = false;
                        ConnectedDrives.Remove(drive);
                    }
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                foreach (var drive in Drives)
                {
                    if (drive.IsConnected)
                    {
                        await _netUseService.DisconnectDrivesMacOSAsync(drive);
                        drive.ButtonColor = "#FF0000"; // Red
                        drive.IsConnected = false;
                        ConnectedDrives.Remove(drive);
                    }
                }
            }

            IsConnected = false;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to disconnect drives: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ConnectSingularDriveAsync(DriveModel drive)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                
                if (drive.IsConnected == false)
                {
                    await _netUseService.ConnectDriveAsync(drive);
                    if (_netUseService.IsError() == false)
                    {
                        ConnectedDrives.Add(drive);
                        drive.ButtonColor = "#00FF00"; // Green
                    }
                    else
                    {
                        drive.ButtonColor = "#FF0000"; // Red
                    }
                    float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                    DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                    DrivePercentage = DriveProgress / 100;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning!",
                        $"Drive {drive.Letter} is already mounted.", "OK");
                }
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            {   
                if (drive.IsConnected == false)
                {
                    await _netUseService.ConnectDriveMacOSAsync(drive);
                    ConnectedDrives.Add(drive);
                    float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                    DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                    DrivePercentage = DriveProgress / 100;
                    drive.ButtonColor = "#00FF00"; // Green
                    drive.IsConnected = true;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning!",
                        $"Drive {drive.Letter} is already mounted.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to connect drive: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DisconnectSingularDriveAsync(DriveModel drive)
    {
        try
        {
            Debug.WriteLine(ConnectedDrives.Count);
            //DriveProgress = 0;
            //DrivePercentage = 0;
            if (OperatingSystem.IsWindows())
            {
                if (drive.IsConnected)
                {
                    await _netUseService.DisconnectDrivesAsync(drive);
                    ConnectedDrives.Remove(drive);
                    float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                    DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                    DrivePercentage = DriveProgress / 100;
                    drive.ButtonColor = "#FF0000"; // Red
                    drive.IsConnected = false;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning!",
                        $"Drive {drive.Letter} is unmounted.", "OK");
                    drive.ButtonColor = "#FF0000"; // Red
                }
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            { 
                if (drive.IsConnected)
                {
                    await _netUseService.DisconnectDrivesMacOSAsync(drive);
                    ConnectedDrives.Remove(drive);
                    float numberOfConnectedDrives = (float)ConnectedDrives.Count;
                    DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                    DrivePercentage = DriveProgress / 100;
                    drive.ButtonColor = "#FF0000"; // Red
                    drive.IsConnected = false;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning!",
                        $"Drive {drive.Letter} is unmounted.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                    $"Unable to disconnect drive: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Views the detail of the drives, IP Address, Drive Letter, userName.
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>

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

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}", true);
    }

    /// <summary>
    /// Goes to the AddDrivePage
    /// </summary>
    /// <returns></returns>

    [RelayCommand]
    private async Task GoToAddDriveAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(AddDrivePage)}", true);
    }


    public ICommand PageAppearingCommand => new Command(async() =>  await GetPopulateList());

    private async Task GetPopulateList()
    {
        if (Drives.Count > 0)
        {
            var drives = await _driveService.GetDriveList();
            var driveList = new List<DriveModel>();

            // Adds each drive into the driveList that was in Drive before clearing it.
            // To check if it was connected or not.
            foreach (var d in Drives) 
            {
                driveList.Add(d);
            }

            Drives.Clear();

            // Checks if the drive was in the driveList, if it was, put the value its IsConnected previous value.
            foreach (var drive in drives)
            {
                var selectedDrive = driveList.Where(d => d.Id == drive.Id).FirstOrDefault();
                if (selectedDrive is not null) 
                {
                    drive.IsConnected = selectedDrive.IsConnected;
                    drive.ButtonColor = "#00FF00"; // Green
                }
                else
                {
                    drive.ButtonColor = "#FF0000"; // Red
                }

                Drives.Add(drive);
            }
        }
    }
}
