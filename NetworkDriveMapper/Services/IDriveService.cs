namespace NetworkDriveMapper.Services;

public interface IDriveService
{
    Task<int> AddDrive(DriveModel drive);
    Task<int> DeleteDrive(DriveModel drive);
    Task<List<DriveModel>> GetDriveList();
    Task<int> UpdateDrive(DriveModel drive);
}