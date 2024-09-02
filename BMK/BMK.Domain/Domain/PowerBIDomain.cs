using BMK.Infrastructure.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using BMK.Models.ViewModels;

namespace BMK.Domain.Domain
{
    public class PowerBIDomain : IPowerBIDomain
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string tenantId;
        private readonly string authority;
        private readonly string workspaceId;
        private readonly string reportId;
        private readonly string username;
        private ILogException LogException { get; set; }

        public PowerBIDomain(IConfiguration configuration, ILogException logException)
        {
            clientId = configuration["PowerBI:clientId"];
            clientSecret = configuration["PowerBI:clientSecret"];
            tenantId = configuration["PowerBI:tenantId"];
            authority = $"{configuration["PowerBI:Authority"]}/{tenantId}";
            workspaceId = configuration["PowerBI:WorkspaceId"];
            reportId = configuration["PowerBI:ReportId"];
            username = configuration["PowerBI:Username"];
            LogException = logException;
        }

        public async Task<Response<PowerBIModel>> GetEmbedToken()
        {
            var response = new Response<PowerBIModel> { IsSucceed = true, Data = new PowerBIModel() };
            bool isFailed = false;
            try
            {
                var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

                var scopes = new string[] { "https://analysis.windows.net/powerbi/api/.default" };
                var authResult = await cca.AcquireTokenForClient(scopes).ExecuteAsync();

                var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");
                using (var client = new PowerBIClient(new Uri("https://api.powerbi.com/"), tokenCredentials))
                {
                    var report = await client.Reports.GetReportInGroupAsync(Guid.Parse(workspaceId), Guid.Parse(reportId));
                    if (report != null)
                    {
                        var generateTokenRequestParameters = new GenerateTokenRequest(
                          accessLevel: "view",
                          datasetId: report.DatasetId
                          //identities: new List<EffectiveIdentity>
                          //{
                          //      new EffectiveIdentity(
                          //          username: username,  // This should be a unique identifier for the user
                          //          roles: new List<string> { "Admin" },  // Replace with actual roles
                          //          datasets: new List<string> { report.DatasetId }  // Use the actual dataset ID
                          //      )
                          //}
                          );

                        var embedToken = await client.Reports.GenerateTokenInGroupAsync(Guid.Parse(workspaceId), report.Id, generateTokenRequestParameters);
                        if (embedToken != null)
                        {
                            response.Data.EmbedToken = Convert.ToString(embedToken.Token);
                            response.Data.ReportId = Convert.ToString(report.Id);
                            response.Data.EmbedUrl = report.EmbedUrl;
                        }
                        else
                            isFailed = true;
                    }
                    else
                        isFailed = true;

                    // var generateTokenRequestParameters = new GenerateTokenRequest("view");
                    // var tokenResponse = await client.Reports.GenerateTokenAsync(new Guid(workspaceId), new Guid(reportId), generateTokenRequestParameters);
                }
            }
            catch (Exception ex)
            {
                response = await AddException(ex, "api/DashBoard/GetEmbedToken", response);
            }
            if (isFailed)
            {
                response = await AddException(null, "api/DashBoard/GetEmbedToken", response);
            }
            return response;
        }

        private async Task<Response<PowerBIModel>> AddException(Exception ex, string url, Response<PowerBIModel> response)
        {
            await LogException.Log(ex, "api/DashBoard/GetEmbedToken");
            response.IsSucceed = false;
            response.Message = "Error occur while retive power bi report";
            return response;
        }
    }

    public interface IPowerBIDomain
    {
        public Task<Response<PowerBIModel>> GetEmbedToken();
    }
}
