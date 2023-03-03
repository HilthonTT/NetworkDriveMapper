namespace NetworkDriveMapper
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(AddDrivePage), typeof(AddDrivePage));
            Routing.RegisterRoute(nameof(UpdatePage), typeof(UpdatePage));
        }
    }
}