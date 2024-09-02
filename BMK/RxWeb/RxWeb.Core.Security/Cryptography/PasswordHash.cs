using System.Security.Cryptography;
using System.Text;

namespace RxWeb.Core.Security.Cryptography
{
    public class PasswordHash : IPasswordHash
    {
        public PasswordHash() {
            
        }

        public PasswordResult Encrypt(string password)
        {

            var result = new PasswordResult();
            result.Salt = RandomNumberGenerator.GetBytes(keySize);
            result.Credential = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            result.Salt,
            iterations,
            hashAlgorithm,
            keySize
                );
            return result;
        }

        public bool VerifySignature(string password, string credential, string salt)
        {
            var saltBytes = Convert.FromHexString(salt);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                iterations,
                hashAlgorithm,
                keySize
                );

            var hashedPasswordToCheck = Convert.ToHexString(hash);

            return hashedPasswordToCheck == credential;
        }
        

        private int keySize = 32;
        private int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    }

    public interface IPasswordHash
    {
       PasswordResult Encrypt(string password);
       bool VerifySignature(string password, string credential, string salt);
    }
}
