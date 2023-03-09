using System.Windows.Input;
using Microsoft.Maui;
using NetworkDriveMapper.Helpers;

namespace NetworkDriveMapper.ViewModels;

public partial class DrivesViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly IDriveMapperService _driveMapperService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILoggedInAppSettings _settings;
    private readonly IConnectorHelper _connectorHelper;

    public DrivesViewModel(IDriveService driveService, 
                            IDriveMapperService driveMapperService,
                            IAppSettingsService appSettingsService,
                            ILoggedInAppSettings settings,
                            IConnectorHelper connectorHelper)
    {
        Title = "Network Drive Mapper";
        _driveService = driveService;
        _driveMapperService = driveMapperService;
        _appSettingsService = appSettingsService;
        _settings = settings;
        _connectorHelper = connectorHelper;
    }

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isConnected;

    private string _searchText;

    public string SearchText
    {
        get { return _searchText; }
        set 
        {
            SetProperty(ref _searchText, value);
            FilterDrives();
        }
    }


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
            IsConnected = true;

            var settings = await _appSettingsService.GetSettings();

            _settings.AutoConnectOnStartUp = settings is null || settings.AutoConnectOnStartUp;

            var drives = await _driveService.GetDriveList();

            if (Drives.Count != 0)
                Drives.Clear();

            foreach (var drive in drives)
            {
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                Drives.Add(drive);
            }

            FilteredDrives = new ObservableCollection<DriveModel>(Drives);

            if (_settings.AutoConnectOnStartUp)
            {
                foreach (var drive in Drives)
                {
                    if (drive.IsConnected is false)
                    {
                        if (OperatingSystem.IsWindows())
                        {
                            await _driveMapperService.ConnectDriveAsync(drive);
                        }
                            
                        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                        {
                            await _driveMapperService.ConnectDriveMacOSAsync(drive);
                        }    
                        else
                            await Shell.Current.DisplayAlert("Error!",
                                ErrorMessage, "OK");

                        if (_driveMapperService.IsError() == false)
                        {
                            drive.ButtonColor = Green;
                            ConnectedDrives.Add(drive);
                        }
                        else
                        {
                            drive.ButtonColor = Red;
                        }

                        float numberOfConnectedDrives = ConnectedDrives.Count;
                        DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                        DrivePercentage = DriveProgress / 100;
                    }
                }
            } 
        }
        catch (Exception ex)
        {
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
            IsConnected = true;

            foreach (var drive in ConnectedDrives)
            {
                drive.IsConnected = false;
            }

            ConnectedDrives.Clear();

            foreach (var drive in Drives)
            {
                if (drive.IsConnected is false)
                {
                    if (OperatingSystem.IsWindows())
                    {
                        await _driveMapperService.ConnectDriveAsync(drive);
                        drive.IsConnected = true;
                        drive.ButtonColor = Green;
                        ConnectedDrives.Add(drive);
                    }

                    else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    {
                        await _driveMapperService.ConnectDriveMacOSAsync(drive);
                        drive.IsConnected = true;
                        drive.ButtonColor = Green;
                        ConnectedDrives.Add(drive);
                    }
                    else
                        await Shell.Current.DisplayAlert("Error!",
                           ErrorMessage, "OK");

                    if (_driveMapperService.IsError() == false)
                    {
                        drive.ButtonColor = Green;
                    }
                    else
                    {
                        drive.ButtonColor = Red;
                    }

                    float numberOfConnectedDrives = ConnectedDrives.Count;
                    DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
                    DrivePercentage = DriveProgress / 100;
                }
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

            await _connectorHelper.DisconnectDrivesAsync(Drives, ConnectedDrives);
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
            await _connectorHelper.ConnectSingularDriveAsync(drive, ConnectedDrives);
            float numberOfConnectedDrives = (float)ConnectedDrives.Count;
            DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
            DrivePercentage = DriveProgress / 100;
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
            await _connectorHelper.DisconnectSingularDriveAsync(drive, ConnectedDrives);
            float numberOfConnectedDrives = (float)ConnectedDrives.Count;
            DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
            DrivePercentage = DriveProgress / 100;

            ConnectedDrives.Remove(drive);
            drive.ButtonColor = Red;
            drive.IsConnected = false;
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
                    if (selectedDrive.IsConnected)
                        drive.ButtonColor = Green;
                    else 
                        drive.ButtonColor = Red;
                }
                else
                {
                    drive.ButtonColor = Red;
                }

                Drives.Add(drive);
            }
        }
    }

    private void FilterDrives()
    {
        FilteredDrives.Clear();
        if (string.IsNullOrWhiteSpace(SearchText) == false)
        {
            foreach (var drive in Drives)
            {

                if (drive.DriveName.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase))
                {
                    FilteredDrives.Add(drive);
                }

            }
        }
        else
        {
            foreach (var drive in Drives)
            {
                FilteredDrives.Add(drive);
            }
        }
    }
}
