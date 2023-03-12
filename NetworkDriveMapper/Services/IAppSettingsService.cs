namespace NetworkDriveMapper.Services;

public interface IAppSettingsService
{
    Task DropTable();
    Task<List<AppSettings>> GetAllSettings();
    Task<AppSettings> GetSettings();
    Task<int> InsertSettings(AppSettings settings);
    Task<int> SaveSettings(AppSettings settings);
}