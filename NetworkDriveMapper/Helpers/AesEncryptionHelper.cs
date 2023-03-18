using System.Security.Cryptography;
using System.Text;
using System;

namespace NetworkDriveMapper.Helpers;

public class AesEncryptionHelper : IAesEncryptionHelper
{
    private readonly Aes _aes;
    public AesEncryptionHelper()
    {
        _aes  = Aes.Create();
        _aes.Key = StringToBytes("PNZaGtW/Z8bpfbv2KxgSEB/ePXWzUqZ3dRoW+ugcb9k=");
        _aes.IV = StringToBytes("eqcMCizAHiTjOX9dnnYrjA==");
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
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using ICryptoTransform decryptor = _aes.CreateDecryptor();
        byte[] decryptedBytes;

        using (var msDecrypt = new MemoryStream(cipherBytes))
        {
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var ms = new MemoryStream();
            await csDecrypt.CopyToAsync(ms);
            decryptedBytes = ms.ToArray();
        }

        string decryptedText = Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0');

        return decryptedText;
    }

    private static byte[] StringToBytes(string text)
    {
        return Convert.FromBase64String(text);
    }
}
