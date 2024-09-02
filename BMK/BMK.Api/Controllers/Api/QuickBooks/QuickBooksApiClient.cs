

using System.Net.Http.Headers;

namespace BMK.Api.Controllers.Api.QuickBooks
{
    internal class QuickBooksApiClient
    {
        private readonly HttpClient _httpClient;
        private HttpClient httpClient;
        private string accessToken;
        private const string ApiBaseUrl = "https://sandbox-quickbooks.api.intuit.com";
        /// <summary>
        /// "https://quickbooks.api.intuit.com";
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="accessToken"></param>

        public QuickBooksApiClient(IHttpClientFactory httpClientFactory, string? accessToken)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        }

        public async Task<string> GetAccountListAsync(long customerID)
        {
            try
            {
                var endpointUrl = $"/v3/company/{customerID}/query?minorversion=14&query=Select * from Account"; //STARTPOSITION 1 MAXRESULTS 100";  Replace {companyId} with the actual company ID

                HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<string> GetBalanceSheetAsync()
        {
            try
            {
                var endpointUrl = "/v3/company/9341451945184343/reports/BalanceSheet?minorversion=14&start_date=2023-10-01&end_date=2023-10-31"; // Replace {companyId} with the actual company ID

                HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        internal string GetRefershTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}