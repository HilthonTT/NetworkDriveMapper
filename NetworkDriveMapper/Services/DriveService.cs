using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDriveMapper.Services;

public class DriveService : IDriveService
{
    private SQLiteAsyncConnection _dbConnection;

    public DriveService()
    {
        SetUpDb();
    }

    private void SetUpDb()
    {
        if (_dbConnection is null)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Drive.db3");
            _dbConnection = new SQLiteAsyncConnection(dbPath);
            _dbConnection.CreateTableAsync<DriveModel>();
        }
    }

    public async Task<int> AddDrive(DriveModel drive)
    {
        return await _dbConnection.InsertAsync(drive);
    }

    public async Task<int> DeleteDrive(DriveModel drive)
    {
        return await _dbConnection.DeleteAsync(drive);
    }

    public async Task<List<DriveModel>> GetDriveList()
    {
        var driveList = await _dbConnection.Table<DriveModel>().ToListAsync();
        return driveList;
    }

    public async Task<int> UpdateDrive(DriveModel drive)
    {
        return await _dbConnection.UpdateAsync(drive);
    }
}
