namespace NetworkDriveMapper.Services;

public class NetUseService : INetUseService
{
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
            await Shell.Current.DisplayAlert("Error!",
                $"Mapping failed with drive {drive.Letter} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
    }

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
            await Shell.Current.DisplayAlert("Error!",
                $"Mapping failed with drive {drive.DriveName} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
    }
}
