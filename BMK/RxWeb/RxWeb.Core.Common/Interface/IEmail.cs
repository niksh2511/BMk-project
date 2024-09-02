namespace RxWeb.Core.Common
{
    public interface IEmail
    {
       Task<bool> SendAsync(MailConfig config);
    }
}
