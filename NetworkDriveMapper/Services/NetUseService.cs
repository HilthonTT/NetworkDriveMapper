namespace NetworkDriveMapper.Services;

public class NetUseService : INetUseService
{
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
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Mapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
            }
        }
        else
        {
            drive.IsConnected = true;
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

        if(process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            if (errorMessage.Contains("local device name is already in use"))
            {
                await Shell.Current.DisplayAlert("Warning!",
                    $"Drive {drive.Letter} is already mounted.", "OK");
                drive.IsConnected = true;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Mapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
                drive.IsConnected = false;
            }
        }
        else
        {
            drive.IsConnected = true;
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
            await Shell.Current.DisplayAlert("Error!",
                $"Failed to unmap {drive.Letter} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
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
            await Shell.Current.DisplayAlert("Error!",
                $"Failed to unmap {drive.Letter} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
    }
}
