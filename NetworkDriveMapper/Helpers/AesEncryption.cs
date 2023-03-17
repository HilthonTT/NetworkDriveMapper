using System.Security.Cryptography;
using System.Text;

namespace NetworkDriveMapper.Helpers;

public class AesEncryption : IAesEncryption
{
    private readonly Aes _aes;
    private readonly AesEncryption _encryption;

    public AesEncryption()
    {
        _aes = Aes.Create();
        _aes.GenerateKey();
        _aes.GenerateIV();
        _encryption = new AesEncryption();
    }

    public async Task<string> EncryptAsync(string plainText)
    {
        byte[] encrypted;

        using (ICryptoTransform encryptor = _aes.CreateEncryptor())
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                await csEncrypt.WriteAsync(plainBytes, 0, plainBytes.Length);
                await csEncrypt.FlushFinalBlockAsync();
            }

            encrypted = msEncrypt.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    public async Task<string> DecryptAsync(string cipherText)
    {
        string decrypted;

        using (ICryptoTransform decryptor = _aes.CreateDecryptor())
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            decrypted = await srDecrypt.ReadToEndAsync();
        }

        return decrypted;
    }

    public async Task<DriveModel> EncryptDriveAsync(DriveModel drive)
    {
        DriveModel encryptedDrive = new()
        {
            Id = drive.Id,
            EncryptedLetter = await _encryption.EncryptAsync(drive.Letter),
            EncryptedAddress = await _encryption.EncryptAsync(drive.Address),
            EncryptedPassword = await _encryption.EncryptAsync(drive.Password),
            EncryptedUserName = await _encryption.EncryptAsync(drive.UserName),
        };

        return encryptedDrive;
    }

    public async Task<DriveModel> DecryptDriveAsync(DriveModel encryptedDrive)
    {
        DriveModel drive = new()
        {
            Id = encryptedDrive.Id,
            Letter = await _encryption.DecryptAsync(encryptedDrive.Letter),
            Address = await _encryption.DecryptAsync(encryptedDrive.Address),
            Password = await _encryption.DecryptAsync(encryptedDrive.Password),
            UserName = await _encryption.DecryptAsync(encryptedDrive.UserName)
        };

        return drive;
    }
}
