using Microsoft.UI.Windowing;
using Microsoft.UI;

namespace NetworkDriveMapper;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

#if WINDOWS
        SetWinNoResizable();
#endif

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
        Routing.RegisterRoute(nameof(AddDrivePage), typeof(AddDrivePage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }

    private void SetWinNoResizable()
    {
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow),
                                                                    (handler, view) =>
        {
#if WINDOWS
            var nativeWindow = handler.PlatformView;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId WindowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(WindowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
#endif
        });
    }
}