namespace NetworkDriveMapper
{
    public partial class App : Application
    {
        private readonly IloggedInAppSettings _loggedInAppSettings;
        private readonly IAppSettingsService _appSettingsService;

        public App(IloggedInAppSettings loggedInAppSettings, IAppSettingsService appSettingsService)
        {
            InitializeComponent();

            MainPage = new AppShell();
            _loggedInAppSettings = loggedInAppSettings;
            _appSettingsService = appSettingsService;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            var settings = await _appSettingsService.GetSettings();
            _loggedInAppSettings.AutoConnectOnStartUp = settings.AutoConnectOnStartUp;
            _loggedInAppSettings.AutoMinimizeAfterConnect = settings.AutoMinimizeAfterConnect;
        }
    }
}