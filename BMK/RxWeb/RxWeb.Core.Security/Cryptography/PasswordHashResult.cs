namespace RxWeb.Core.Security.Cryptography
{
    public class PasswordResult
    {

        public byte[] Salt { get; set; }
        public byte[] Credential { get; set; }
    }
}
