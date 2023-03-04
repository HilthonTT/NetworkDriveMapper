namespace NetworkDriveMapper.Services;

public class NetUseService : INetUseService
{
    public async Task ConnectDriveAsync(string driveLetter,
                                        string address,
                                        string driveName,
                                        string password,
                                        string userName)
    {
        Process process = new();
        process.StartInfo.FileName = "net";
        process.StartInfo.Arguments = $"use {driveLetter}: \"\\\\{address}\\{driveName}\" {password} /user:{userName} /persistent:no";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            await Shell.Current.DisplayAlert("Error!",
                $"Mapping failed with drive {driveLetter} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
    }

    public async Task ConnectDriveMacOSAsync(string address,
                                        string driveName,
                                        string password,
                                        string userName)
    {
        Process process = new();
        process.StartInfo.FileName = "sudo";
        process.StartInfo.Arguments = $"-t smbfs //{userName}:{password}@{address}/share {driveName}";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        await process.WaitForExitAsync(); // Wait for the command to complete asynchronously

        string errorMessage = process.StandardError.ReadToEnd();

        if (process.ExitCode is not 0)
        {
            // Handle the case where the command failed
            await Shell.Current.DisplayAlert("Error!",
                $"Mapping failed with drive {driveName} with error code {process.ExitCode}: {errorMessage}", "OK");
        }
    }
}
