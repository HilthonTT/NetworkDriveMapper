using SQLite;

namespace NetworkDriveMapper.Model;

public class DriveModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Letter { get; set; }
    public string Address { get; set; }
    public string DriveName { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}
