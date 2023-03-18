using Microsoft.Win32;
using NetworkDriveMapper.Helpers;
using System.Reflection;

namespace NetworkDriveMapper.ViewModels;

public partial class DrivesViewModel : BaseViewModel
{
    private readonly IDriveService _driveService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILoggedInAppSettings _settings;
    private readonly IConnectorHelper _connectorHelper;
    private readonly IAesEncryptionHelper _encryption;

    public DrivesViewModel(IDriveService driveService,
                            IAppSettingsService appSettingsService,
                            ILoggedInAppSettings settings,
                            IConnectorHelper connectorHelper,
                            IAesEncryptionHelper encryption)
    {
        Title = "Network Drive Mapper";
        _driveService = driveService;
        _appSettingsService = appSettingsService;
        _settings = settings;
        _connectorHelper = connectorHelper;
        _encryption = encryption;
    }

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _searchText;

    partial void OnSearchTextChanged(string value)
    {
        FilterDrives();
    }

    [ObservableProperty]
    private float _driveProgress = 0; // Progress Label, shows the percentage in number

    [ObservableProperty]
    private float _drivePercentage; // The progress bar percentage, changes the progress bar's length.

    /// <summary>
    /// This method is called on startup.
    /// It checks if the setting is set to AutoConnect,
    /// if it is, connect the drives if not don't.
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
                await DecryptDrive(drive);
                await _connectorHelper.ChecksForConnectedDrivesAsync(drive, ConnectedDrives);
                Drives.Add(drive);
                RecalculateProgressbar();
            }

            FilteredDrives = new ObservableCollection<DriveModel>(Drives);

            if (_settings.AutoConnectOnStartUp)
            {
                await ConnectDisconnectedDrivesAsync();
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

            await ConnectDisconnectedDrivesAsync();  
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to map drives: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DisconnectAllDrivesAsync()
    {
        try
        {
            await DisconnectConnectedDrivesAsync();
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
            await _connectorHelper.ConnectDriveAsync(drive, ConnectedDrives);
            RecalculateProgressbar();
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
            await _connectorHelper.DisconnectDriveAsync(drive, ConnectedDrives);
            RecalculateProgressbar();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!",
                    $"Unable to disconnect drive: {ex.Message}", "OK");
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
    /// This method basically reloads the Drives List on the main page
    /// to see if there were any modification
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task PageAppearingAsync()
    {
        var drives = await _driveService.GetDriveList();
        if (Drives.Count != drives.Count)
        {
            var driveList = new List<DriveModel>();

            // Adds each drive into the driveList that was in Drive before clearing it.
            // To check if it was connected or not in the later lines.
            foreach (var d in Drives) 
            {
                driveList.Add(d);
            }

            Drives.Clear();
            FilteredDrives.Clear();

            // Checks if the drive was in the driveList, if it was, put the value its IsConnected previous value.
            foreach (var drive in drives)
            {
                await DecryptDrive(drive);
                var selectedDrive = driveList.Where(d => d.Id == drive.Id).FirstOrDefault();
                if (selectedDrive is not null) 
                {
                    if (selectedDrive.IsConnected)
                        SetPropertyToConnected(drive);
                    else 
                        SetPropertyToDisconnected(drive);
                }
                else
                {
                    SetPropertyToDisconnected(drive);
                }

                Drives.Add(drive);
                FilteredDrives.Add(drive);
            }
        }
    }

    private async Task FilterDrives()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        FilteredDrives.Clear();
        if (string.IsNullOrWhiteSpace(SearchText) == false)
        {
            var filtered = await Task.Run(() =>
                Drives.Where(drive =>
                    drive.DriveName.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) ||
                    drive.Letter.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)));

            foreach (var drive in filtered)
            {
                FilteredDrives.Add(drive);
                await Task.Yield();
            }
        }
        else
        {
            foreach (var drive in Drives)
            {
                FilteredDrives.Add(drive);
                await Task.Yield();
            }
        }
        IsBusy = false;
    }

    private async Task ConnectDisconnectedDrivesAsync()
    {
        var disconnectedDrives = Drives.Where(d => d.IsConnected is false);
        foreach (var drive in disconnectedDrives)
        {
            await _connectorHelper.ConnectDriveAsync(drive, ConnectedDrives);
            RecalculateProgressbar();
        }
    }

    private async Task DisconnectConnectedDrivesAsync()
    {
        var connectedDrives = Drives.Where(d => d.IsConnected);
        foreach (var drive in connectedDrives)
        {
            await _connectorHelper.DisconnectDriveAsync(drive, ConnectedDrives);
            RecalculateProgressbar();
        }
    }

    private void RecalculateProgressbar()
    {
        float numberOfConnectedDrives = ConnectedDrives.Count;
        DriveProgress = (numberOfConnectedDrives / Drives.Count) * 100;
        DrivePercentage = DriveProgress / 100;
    }

    private static void SetPropertyToConnected(DriveModel drive)
    {
        drive.ButtonColor = Green;
        drive.IsConnected = true;
    }

    private static void SetPropertyToDisconnected(DriveModel drive)
    {
        drive.ButtonColor = Red;
        drive.IsConnected = false;
    }

    private async Task DecryptDrive(DriveModel drive)
    {
        drive.Letter = await _encryption.DecryptAsync(drive.Letter);
        drive.Address = await _encryption.DecryptAsync(drive.Address);
        drive.DriveName = await _encryption.DecryptAsync(drive.DriveName);
        drive.Password = await _encryption.DecryptAsync(drive.Password);
        drive.UserName = await _encryption.DecryptAsync(drive.UserName);
    }
}