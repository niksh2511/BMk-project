using RxWeb.Core.Common.Models;
using BMK.Infrastructure.Logs;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

using static System.Net.WebRequestMethods;

namespace RxWeb.Core.Common.Sms
{
    public class TwilioSms : ITextSms
    {
        private ILogException LogException { get; set; }
        public TwilioSms(TwilioSmsConfiguration smsConfiguration,ILogException logException)
        {
            SmsConfiguration = smsConfiguration;
            LogException = logException;
        }
        public async Task<VerificationResource> SendAsync(SmsConfig smsConfig)
        {
            try
            {
                TwilioClient.Init(SmsConfiguration.AccountSid, SmsConfiguration.AuthToken);
                var verification = await VerificationResource.CreateAsync(
                   pathServiceSid: SmsConfiguration.PathServiceId,
                   to: smsConfig.To,
                   channel: "sms"
                );
                return verification;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "An error occurred while sending the OTP. Please try again.");
                return null;
            }

        }

        public async Task<string> VerfiyAsync(string toPhoneNo, string otp)
        {
            try
            {
                TwilioClient.Init(SmsConfiguration.AccountSid, SmsConfiguration.AuthToken);
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: toPhoneNo,
                    pathServiceSid: SmsConfiguration.PathServiceId,
                    code: otp
                );
                bool isVaild = verificationCheck.Valid ?? false;
                string status = isVaild ? "approved" : "invalid";
                return status;
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "An error occurred while verify the OTP. Please try again.");
                return "expired";
            }

        }

        public TwilioSmsConfiguration SmsConfiguration { get; set; }
    }
}
