using BMK.Api.Bootstrap;
using BMK.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using RxWeb.Core.AspNetCore.Extensions;
using SoapCore;

namespace BMK.Api
{
    public static class Startup
    {
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddConfigurationOptions(configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddPerformance();
            builder.Services.AddSecurity(configuration);
            builder.Services.AddSingletonService();
            builder.Services.AddSoapCore();
            builder.Services.AddScopedService(configuration);
            builder.Services.AddDbContextService();
            builder.Services.AddRxWebLocalization();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddSwaggerOptions();
            //builder.Services.AddMvcCore(options =>
            //{
            //    options.InputFormatters.Insert(0, new XmlSerializerInputFormatter(new MvcOptions()));
            //    options.OutputFormatters.Insert(0, new XmlSerializerOutputFormatter());
            //});
            builder.Services.AddMvc(options =>
            {
                //options.AddRxWebSanitizers();
                options.AddValidation();
               
            })
                .AddNewtonsoftJson(
                oo =>
                {
                    var resolver = new CamelCasePropertyNamesContractResolver();
                    if (resolver != null)
                    {
                        var res = resolver as DefaultContractResolver;
                        res.NamingStrategy = null;
                    }
                    oo.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
            });

            return builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static async Task ConfigureAsync(this WebApplication app)
        {
            app.UsePerformance();

            app.UseRouting();
            app.UseSession();
            app.UseSecurity(app.Environment);

            app.UseStaticFiles();

            app.MapControllers();
            //app.MapFallbackToFile("index.html");
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<IQuickBooksService>("/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
            });
            await app.RunAsync();
        }
    }
}



