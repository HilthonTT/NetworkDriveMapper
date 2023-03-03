using Microsoft.Extensions.Logging;

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

        // View Models
        builder.Services.AddSingleton<DrivesViewModel>();
        builder.Services.AddTransient<DetailsViewModel>();
        builder.Services.AddTransient<AddViewModel>();
        builder.Services.AddTransient<UpdateViewModel>();

        // Views Registration
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddTransient<AddDrivePage>();
        builder.Services.AddTransient<UpdatePage>();

        return builder.Build();
    }
}