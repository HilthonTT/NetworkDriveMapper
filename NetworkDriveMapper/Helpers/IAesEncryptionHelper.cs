namespace NetworkDriveMapper.Helpers
{
    public interface IAesEncryptionHelper
    {
        Task<string> DecryptAsync(string cipherText);
        Task<string> EncryptAsync(string plainText);
    }
}