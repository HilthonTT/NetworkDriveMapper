namespace NetworkDriveMapper.Services;

public class DriveMapperService : IDriveMapperService
{
    private string ErrorMessage = "";
    private const string Green = "#00FF00";
    private const string Red = "#FF0000";

    public bool IsError()
    {
        if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
        {
            return true;
        }

        return false;
    }


    public async Task ChecksForConnectedDrivesAsync(DriveModel drive)
    {
        string driveLetter = drive.Letter + ":";

        bool isConnected = await Task.Run(() => Directory.Exists(driveLetter));

        if (isConnected)
        {
            drive.IsConnected = true;
            drive.ButtonColor = Green;
            ErrorMessage = "";
        }
        else
        {
            drive.IsConnected = false;
            drive.ButtonColor = Red;
            ErrorMessage = "Drive is not connected";
        }
    }

    public async Task ChecksForConnectedDrivesMacOSAsync(DriveModel drive)
    {
        Process process = new Process();
        process.StartInfo.FileName = "diskutil";
        process.StartInfo.Arguments = "list -plist";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        
        await process.WaitForExitAsync();

        bool isMounted = output.Contains(drive.DriveName);

        if (isMounted)
        {
            drive.IsConnected = true;
            drive.ButtonColor = Green;
        }
        else
        {
            drive.IsConnected = false;
            drive.ButtonColor = Red;
        }
    }

    /// <summary>
    /// Maps the drive on windows.
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>
    public async Task ConnectDriveAsync(DriveModel drive)
    {
        Process process = new();
        process.StartInfo.FileName = "net";
        process.StartInfo.Arguments = $"use {drive.Letter}: \"\\\\{drive.Address}\\{drive.DriveName}\" {drive.Password} /user:{drive.UserName} /persistent:no";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            if (errorMessage.Contains("local device name is already in use"))
            {
                await Shell.Current.DisplayAlert("Warning!",
                    $"Drive {drive.Letter} is already mounted.", "OK");
                drive.IsConnected = true;
                drive.ButtonColor = Green;
                ErrorMessage = "";
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Mapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = errorMessage;
            }
        }
        else
        {
            drive.IsConnected = true;
            drive.ButtonColor = Green;
            ErrorMessage = "";
        }
    }

    /// <summary>
    /// Maps the drive on MacOS or MacCatalyst
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>
    public async Task ConnectDriveMacOSAsync(DriveModel drive)
    {
        Process process = new();
        process.StartInfo.FileName = "sudo";
        process.StartInfo.Arguments = $"-t smbfs //{drive.UserName}:{drive.Password}@{drive.Address}/share {drive.DriveName}";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            if (errorMessage.Contains("local device name is already in use"))
            {
                await Shell.Current.DisplayAlert("Warning!",
                    $"Drive {drive.Letter} is already mounted.", "OK");
                drive.IsConnected = true;
                drive.ButtonColor = Green;
                ErrorMessage = "";
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Mapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = errorMessage;
            }
        }
        else
        {
            drive.IsConnected = true;
            drive.ButtonColor = Green;
            ErrorMessage = "";
        }
    }

    /// <summary>
    /// Unmaps the drive on windows.
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>
    public async Task DisconnectDrivesAsync(DriveModel drive)
    {
        Process process = new();
        process.StartInfo.FileName = "net";
        process.StartInfo.Arguments = $"use {drive.Letter}: /del";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            if (errorMessage.Contains("The network connection could not be found."))
            {
                await Shell.Current.DisplayAlert("Warning!",
                    $"Drive {drive.Letter} is already unmapped.", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = "";
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Unmapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = errorMessage;
            }
        }

        drive.ButtonColor = Red;
        ErrorMessage = "";
    }

    /// <summary>
    /// Unmaps the drive on Mac or MacCatalyst
    /// </summary>
    /// <param name="drive"></param>
    /// <returns></returns>
    public async Task DisconnectDrivesMacOSAsync(DriveModel drive)
    {
        Process process = new();
        process.StartInfo.FileName = "sudo";
        process.StartInfo.Arguments = $"umount {drive.DriveName}";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            if (errorMessage.Contains("The network connection could not be found."))
            {
                await Shell.Current.DisplayAlert("Warning!",
                    $"Drive {drive.Letter} is already unmapped.", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = "";
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Unmapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
                drive.ButtonColor = Red;
                ErrorMessage = errorMessage;
            }
        }

        drive.ButtonColor = Red;
        ErrorMessage = "";
    }
}
