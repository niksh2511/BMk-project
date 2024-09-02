using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RxWeb.Core.Common.Models;
using RxWeb.Core.Common.Sms;

namespace RxWeb.Core.Common.Extensions
{
    public static class SmsServiceExtension
    {
        public static void AddSmsService(this IServiceCollection serviceCollection, IConfiguration configuration) {
            Configure(configuration.GetSection(SMSTYPE).Value, serviceCollection, configuration);
        }
        private static void Configure(string smsType, IServiceCollection serviceCollection, IConfiguration configuration)
        {
            smsType = TWILIO;
            switch (smsType)
            {
                case TWILIO:
                    serviceCollection.AddScoped<ITextSms, TwilioSms>();
                    var twilioSmsSection = configuration.GetSection("TwilioSms");
                    var twilioSmsConfiguration = new TwilioSmsConfiguration();
                    twilioSmsSection.Bind(twilioSmsConfiguration);
                    serviceCollection.AddSingleton<TwilioSmsConfiguration>(twilioSmsConfiguration);
                    break;
            }
        }

        const string TWILIO = "Twilio";

        const string SMSTYPE = "SmsType";
    }
}
