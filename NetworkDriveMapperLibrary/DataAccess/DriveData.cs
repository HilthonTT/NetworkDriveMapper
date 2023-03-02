using NetworkDriveMapperLibrary.Internal.DataAccess;
using NetworkDriveMapperLibrary.Models;

namespace NetworkDriveMapperLibrary.DataAccess;

public class DriveData
{
    private readonly ISqliteDataAccess _sql;

    public DriveData(ISqliteDataAccess sql)
    {
        _sql = sql;
    }

    public List<DriveModel> GetDrives()
    {
        var output = _sql.LoadData<DriveModel>("Default", "select * from Drive");

        return output;
    }

    public void SaveDrive(DriveModel drive)
    {
        _sql.SaveData(drive, "Default", "insert into Drive (Letter, Address, Name, Password, UserName) values " +
                    "(@Letter, @Address, @Name, @Password, @UserName)");
    }
}
