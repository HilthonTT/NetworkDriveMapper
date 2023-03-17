namespace NetworkDriveMapper.Helpers
{
    public interface IAesEncryption
    {
        Task<string> DecryptAsync(string cipherText);
        Task<DriveModel> DecryptDriveAsync(DriveModel encryptedDrive);
        Task<string> EncryptAsync(string plainText);
        Task<DriveModel> EncryptDriveAsync(DriveModel drive);
    }
}