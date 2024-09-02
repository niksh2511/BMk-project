namespace RxWeb.Core.Common.Models
{
    public class TwilioSmsConfiguration
    {
        public string AccountSid { get; set; }

        public string AuthToken { get; set; }
        public string FromPhoneNo { get; set; }

        public string CountryCode { get; set; }

        public string PathServiceId { get; set; }
    }
}
