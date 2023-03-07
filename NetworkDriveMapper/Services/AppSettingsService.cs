using SQLite;

namespace NetworkDriveMapper.Services;

public class AppSettingsService : IAppSettingsService
{
    private SQLiteAsyncConnection _dbConnection;

    public AppSettingsService()
    {
        SetUpDb();
    }

    private void SetUpDb()
    {
        if (_dbConnection is null)
        {
            string dbPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), 
                        "AppSettings.db3");
            _dbConnection = new SQLiteAsyncConnection(dbPath);
            _dbConnection.CreateTableAsync<AppSettings>();
        }
    }

    public async Task<int> SaveSettings(AppSettings settings)
    {
        return await _dbConnection.UpdateAsync(settings);
    }

    public async Task<int> InsertSettings(AppSettings settings)
    {
        return await _dbConnection.InsertAsync(settings);
    }

    public async Task<List<AppSettings>> GetAllSettings()
    {
        return await _dbConnection.Table<AppSettings>().ToListAsync();
    }

    public async Task<AppSettings> GetSettings()
    {
        var settings = await _dbConnection.Table<AppSettings>().ToListAsync();
        return settings.FirstOrDefault();
    }
}