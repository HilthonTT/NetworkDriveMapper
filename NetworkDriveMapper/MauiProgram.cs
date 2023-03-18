using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetworkDriveMapper.Helpers;

namespace NetworkDriveMapper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IDriveService, DriveService>();
        builder.Services.AddSingleton<IDriveMapperService, DriveMapperService>();
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
        builder.Services.AddSingleton<ILoggedInAppSettings, LoggedInAppSettings>();
        builder.Services.AddSingleton<IConnectorHelper, ConnectorHelper>();

        // Configuration JSON File Injection
        builder.Configuration.AddConfiguration(AddConfiguration());

        // Encryption Service
        builder.Services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();
        
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        // View Models
        builder.Services.AddSingleton<DrivesViewModel>();
        builder.Services.AddTransient<DetailsViewModel>();
        builder.Services.AddTransient<AddViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Views Registration
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddTransient<AddDrivePage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }

    private static IConfiguration AddConfiguration()
    {
        string configFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);


        return builder.Build();
    }
}