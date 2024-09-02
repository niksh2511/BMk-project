using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RxWeb.Core.Security.Cryptography
{
    public class AesEncryption : IAesEncryption
    {
        private readonly IConfiguration Config;
        public AesEncryption(IConfiguration config)
        {
            Config = config;
        }
        public string Encrypt(string plainText)
        {
            try
            {
                string EncryptionKey = Config["BmkSetting:EncryptionKey"];
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Generate an IV (Initialization Vector) for CBC mode
                    aesAlg.GenerateIV();
                    byte[] iv = aesAlg.IV;

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv))
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                cs.Write(plainBytes, 0, plainBytes.Length);
                                cs.FlushFinalBlock();
                            }

                            // Combine IV and encrypted data
                            byte[] encryptedData = ms.ToArray();
                            byte[] combinedData = new byte[iv.Length + encryptedData.Length];
                            Array.Copy(iv, combinedData, iv.Length);
                            Array.Copy(encryptedData, 0, combinedData, iv.Length, encryptedData.Length);

                            // Return the Base64 representation of the encrypted data
                            return Convert.ToBase64String(combinedData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddExceptionLog(ex, "Error Occur in Encryption");
                return string.Empty;
            }
        }
        public string Decrypt(string encryptedText)
        {
            try
            {
                string EncryptionKey = Config["BmkSetting:EncryptionKey"];
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Extract IV from the encrypted data
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    byte[] encryptedData = new byte[encryptedBytes.Length - iv.Length];
                    Array.Copy(encryptedBytes, iv, iv.Length);
                    Array.Copy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

                    aesAlg.IV = iv;

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                            {
                                cs.Write(encryptedData, 0, encryptedData.Length);
                                cs.FlushFinalBlock();
                            }

                            byte[] decryptedBytes = ms.ToArray();
                            return Encoding.UTF8.GetString(decryptedBytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddExceptionLog(ex, "Error Occur in Decryption");
                return string.Empty;
            }
        }

        public string GenerateActivationKey()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] keyChars = new char[10];

            for (int i = 0; i < 10; i++)
            {
                keyChars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(keyChars);
        }
        public void AddExceptionLog(Exception ex, string Url)
        {

        }

    }

    public interface IAesEncryption
    {
        string Encrypt(string plainText);

        string Decrypt(string encryptedText);
        string GenerateActivationKey();
    }
}
