using Twilio.Rest.Verify.V2.Service;

namespace RxWeb.Core.Common
{
    public interface ITextSms
    {
        Task<VerificationResource> SendAsync(SmsConfig smsConfig);
        Task<string> VerfiyAsync(string toPhoneNo, string otp);
    }
}
