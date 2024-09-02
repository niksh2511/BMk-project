using System.Net.Http.Headers;
using BMK.Models.DbEntities;
using Microsoft.AspNetCore.Http;
using RxWeb.Core.Security;
using BMK.UnitOfWork.Main;
using Microsoft.Extensions.Configuration;
using BMK.Infrastructure.Model;
using BMK.Infrastructure.Singleton;
using BMK.Models.ViewModels;
using System.Net.Http.Json;
using Microsoft.Data.SqlClient;
using RxWeb.Core.Data;
using BMK.BoundedContext.SqlDbContext;
using RxWeb.Core.Security.Cryptography;
using BMK.Infrastructure.Logs;

namespace BMK.Domain.Domain
{
    public class QuickBookDomain : IQuickBookDomain
    {
        private IHttpClientFactory HttpClientFactory { get; set; }
        private ISessionProvider SessionProvider { get; set; }
        private IDbContextManager<MainSqlDbContext> DbContextManager { get; set; }
        private IQBUow Uow { get; set; }
        private readonly IUserClaim UserClaim;
        private readonly IConfiguration Config;
        private IPasswordHash PasswordHash { get; set; }
        private ILogException LogException { get; set; }
        public QuickBookDomain(IUserClaim userClaim, IQBUow uow, IConfiguration config, ISessionProvider sessionProvider, IHttpClientFactory httpClientFactory, IDbContextManager<MainSqlDbContext> dbContextManager, IPasswordHash passwordHash, ILogException logException)
        {
            UserClaim = userClaim;
            Uow = uow;
            Config = config;
            SessionProvider = sessionProvider;
            HttpClientFactory = httpClientFactory;
            DbContextManager = dbContextManager;
            PasswordHash = passwordHash;
            LogException = logException;
        }

        //Method For the Quickbook Online user
        public async Task<string> Connect()
        {
            await setQuickBoookSession();
            return $"https://appcenter.intuit.com/connect/oauth2?client_id={Config["QuickBooksOAuth:ClientId"]}&redirect_uri={Config["QuickBooksOAuth:CallbackPath"]}&response_type=code&scope=com.intuit.quickbooks.accounting&state=HCPOV";
        }
        public async Task<string> CheckQbStatus()
        {
            Response<object> qbResponse = new Response<object> { Message = "Not Connected", IsSucceed = false };
            QbTokenDetail qbDetails = await Uow.Repository<QbTokenDetail>().SingleOrDefaultAsync(x => x.QbUserId == UserClaim.UserId);
            await setQuickBoookSession();
            if (qbDetails != null)
            {
                if (string.IsNullOrEmpty(qbDetails.QbAccessToken))
                {
                    return qbResponse.Message;
                }
                if (qbDetails.QbExpireTime <= DateTime.Now)
                {
                    qbResponse = await GetRefershTokenAsync(qbDetails);
                }
                else
                {
                    qbResponse.Message = "Connected";
                    return qbResponse.Message;
                }
            }
            return qbResponse.Message;
        }
        public async Task<Response<object>> Callback(string code = "", string state = "", string customerId = "", string url = "", bool isRefreshToken = false, string refreshToken = "")
        {
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            Response<object> qbResponse = new Response<object>();
            try
            {
                string tokenEndpoint = Convert.ToString(Config["QuickBooksOAuth:TokenEndPoint"]);
                Dictionary<string, string> headerOptions;
                if (!isRefreshToken)
                {
                    headerOptions = new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code"},
                        { "code", code},
                        { "redirect_uri", Convert.ToString(Config["QuickBooksOAuth:CallbackPath"]) },
                        { "client_id", Convert.ToString(Config["QuickBooksOAuth:ClientId"]) },
                        { "client_secret", Convert.ToString(Config["QuickBooksOAuth:ClientSecret"]) }
                    };
                }
                else
                {
                    headerOptions = new Dictionary<string, string>
                    {
                        { "grant_type", "refresh_token"},
                        { "refresh_token", refreshToken },
                        { "client_id", Convert.ToString(Config["QuickBooksOAuth:ClientId"]) },
                        { "client_secret", Convert.ToString(Config["QuickBooksOAuth:ClientSecret"]) }
                    };
                }
                var content = new FormUrlEncodedContent(headerOptions);
                HttpClient HttpClient = HttpClientFactory.CreateClient();
                var response = await HttpClient.PostAsync(tokenEndpoint, content);
                response.EnsureSuccessStatusCode();
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokenResponse != null)
                {
                    if (!isRefreshToken)
                    {
                        await AddQBProccesLog("Quickbook Online User Connected!!");
                    }
                    QbTokenDetail qbDetails = await Uow.Repository<QbTokenDetail>().SingleOrDefaultAsync(x => x.QbUserId == userInfo.UserId);
                    QbTokenDetail qbTokenDetail = new QbTokenDetail();
                    qbTokenDetail.QbUserId = userInfo.UserId;
                    qbTokenDetail.QbCustomerId = isRefreshToken ? qbDetails.QbCustomerId : long.Parse(customerId);
                    qbTokenDetail.QbRefreshToken = tokenResponse.refresh_Token;
                    qbTokenDetail.QbAccessToken = tokenResponse.Access_Token;
                    qbTokenDetail.QbExpireTime = Convert.ToDateTime(DateTime.Now.AddSeconds(tokenResponse.expires_in));
                    qbTokenDetail.QbRefreshTokenExpireTime = Convert.ToDateTime(DateTime.Now.AddSeconds(tokenResponse.x_refresh_token_expires_in));

                    if (qbDetails != null)
                    {
                        qbTokenDetail.QbId = qbDetails.QbId;
                        await Uow.RegisterDirtyAsync(qbTokenDetail);
                    }
                    else
                    {
                        await Uow.RegisterNewAsync(qbTokenDetail);
                    }
                    await Uow.CommitAsync();

                }
                qbResponse.IsSucceed = true;
                qbResponse.Message = "Connected";
                return qbResponse;
            }
            catch (HttpRequestException ex)
            {
                await AddQBProccesLog("Web Connector Authenticated Failed!");
                await QbExceptionLog(ex.Message, "QuickBookDomain.Callback", url, userInfo);
            }
            catch (Exception ex)
            {
                await AddQBProccesLog("Web Connector Authenticated Failed!");
                await QbExceptionLog(ex.Message, "QuickBookDomain.Callback", url, userInfo);
            }
            qbResponse.IsSucceed = false;
            qbResponse.Message = "Connection Failed";
            return qbResponse;
        }
        public async Task<Response<object>> RetrieveQbData()
        {
            Response<object> response = new Response<object>();
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            QbTokenDetail qbDetails = await Uow.Repository<QbTokenDetail>().SingleOrDefaultAsync(x => x.QbUserId == UserClaim.UserId);
            if (qbDetails != null)
            {
                if (string.IsNullOrEmpty(qbDetails.QbAccessToken))
                {
                    response.IsSucceed = true;
                    response.Message = "Disconnected";
                    return response;
                }

                if (qbDetails.QbExpireTime <= DateTime.Now)
                {
                    //refresh token 
                    await GetRefershTokenAsync(qbDetails);
                }

                try
                {
                    response = await GetAccountListAsync((long)qbDetails.QbCustomerId, qbDetails.QbAccessToken);
                    if (response.IsSucceed)
                    {
                        await RetrieveFiveYearBalanceSheet(qbDetails.QbAccessToken);//need to manage if the error occur during process
                        response.Message = "QuickBooks import data successfully";
                    }
                    else
                    {
                        response.Message = "Error occured during for collecting QuickBook data";
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    await QbExceptionLog(ex.Message, "QuickBookDomain.RetrieveQbData", "api/QuickBooks/retrieveQbData", userInfo);
                    response.IsSucceed = false;
                    response.Message = "Error occured during for collecting Account Details/Balance sheet data";
                    return response;
                }
            }
            else
            {
                response.IsSucceed = false;
                response.Message = "Reconnect With QuickBook";
            }
            return response;
        }
        public async Task<Response<object>> GetAccountListAsync(long customerID, string accessToken)
        {
            Response<object> qbResponse = new Response<object>();
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            try
            {
                var endpointUrl = $"/v3/company/{customerID}/query?minorversion=14&query=Select * from Account STARTPOSITION 1 MAXRESULTS 1000"; //STARTPOSITION 1 MAXRESULTS 100";  Replace {companyId} with the actual company ID
                HttpClient httpClient = InitializeHttpClient(HttpClientFactory, accessToken);
                await AddQBProccesLog("Requesting Charter of Accounts");
                HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();
                var accountInfoJson = await response.Content.ReadAsStringAsync();
                await AddQBProccesLog("Response quickbook online accounts", accountInfoJson);
                var spParameters = new SqlParameter[4];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                spParameters[2] = new SqlParameter() { ParameterName = "accountListXML", Value = accountInfoJson };
                spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBO" };
                var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountListUpdateV2", spParameters);
                qbResponse.IsSucceed = true;

            }
            catch (HttpRequestException ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetAccountListAsync", "api/QuickBooks/retrieveQbData", userInfo);
                qbResponse.IsSucceed = false;
                qbResponse.Message = "Error occured during for collecting account details";
            }
            catch (Exception ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetAccountListAsync", "api/QuickBooks/retrieveQbData", userInfo);
                qbResponse.IsSucceed = false;
                qbResponse.Message = "Error occured during for collecting account details";
            }
            return qbResponse;
        }
        public async Task RetrieveFiveYearBalanceSheet(string accessToken)
        {
            QbTokenDetail qbDetails = await Uow.Repository<QbTokenDetail>().SingleOrDefaultAsync(x => x.QbUserId == UserClaim.UserId);
            List<(string start, string end, string monthYear)> dateRanges = GetBalanceSheetDateList();
            foreach (var dateRange in dateRanges)
            {
                await GetMonthWiseBalanceSheetDataAsync((long)qbDetails.QbCustomerId, dateRange.start, dateRange.end, qbDetails.QbAccessToken);
                await GetMonthWiseProfitLossDataAsync((long)qbDetails.QbCustomerId, dateRange.start, dateRange.end, qbDetails.QbAccessToken);
            }
        }
        public async Task<Response<object>> GetMonthWiseBalanceSheetDataAsync(long customerId, string startDate, string endDate, string accesstoken)
        {
            Response<object> qbResponse = new Response<object>();
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            var endpointUrl = $"/v3/company/{customerId}/reports/BalanceSheet?minorversion=14&start_date={startDate}&end_date={endDate}&accounting_method=Accrual"; // Replace {companyId} with the actual company ID
            try
            {
                await AddQBProccesLog($"Requesting balance of accounts for the period from {startDate} to {endDate}");
                HttpClient httpClient = InitializeHttpClient(HttpClientFactory, accesstoken);
                HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();
                var balanceInfoJson = await response.Content.ReadAsStringAsync();
                await AddQBProccesLog($"Response balance of accounts for the period from {startDate} to {endDate}", balanceInfoJson);
                var spParameters = new SqlParameter[4];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                spParameters[2] = new SqlParameter() { ParameterName = "accountBalanceXML", Value = balanceInfoJson };
                spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBO" };
                var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountUpdateBSV2", spParameters);
                return qbResponse;

            }
            catch (HttpRequestException ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetMonthWiseBalanceSheetDataAsync", endpointUrl, userInfo);
                return qbResponse;
            }
            catch (Exception ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetMonthWiseBalanceSheetDataAsync", endpointUrl, userInfo);
                return qbResponse;
            }
        }

        public async Task<Response<object>> GetMonthWiseProfitLossDataAsync(long customerId, string startDate, string endDate, string accesstoken)
        {
            Response<object> qbResponse = new Response<object>();
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            var endpointUrl = $"/v3/company/{customerId}/reports/ProfitAndLoss?minorversion=14&start_date={startDate}&end_date={endDate}&accounting_method=Accrual"; // Replace {companyId} with the actual company ID
            try
            {
                await AddQBProccesLog($"Requesting balance of accounts for the period from {startDate} to {endDate}");
                HttpClient httpClient = InitializeHttpClient(HttpClientFactory, accesstoken);
                HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);
                response.EnsureSuccessStatusCode();
                var balanceInfoJson = await response.Content.ReadAsStringAsync();
                await AddQBProccesLog($"Response balance of accounts for the period from {startDate} to {endDate}", balanceInfoJson);
                var spParameters = new SqlParameter[4];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                spParameters[2] = new SqlParameter() { ParameterName = "accountBalanceXML", Value = balanceInfoJson };
                spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBO" };
                var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountUpdatePLV2", spParameters);
                return qbResponse;

            }
            catch (HttpRequestException ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetMonthWiseBalanceSheetDataAsync", endpointUrl, userInfo);
                return qbResponse;
            }
            catch (Exception ex)
            {
                await QbExceptionLog(ex.Message, "QuickBookDomain.GetMonthWiseBalanceSheetDataAsync", endpointUrl, userInfo);
                return qbResponse;
            }
        }
        public async Task<Response<object>> GetRefershTokenAsync(QbTokenDetail qbDetails)
        {
            Response<object> qbResponse = new Response<object> { Message = "Disconnected", IsSucceed = true };
            if (qbDetails.QbRefreshTokenExpireTime >= DateTime.Now)
            {
                qbResponse = await Callback(isRefreshToken: true, refreshToken: qbDetails.QbRefreshToken);
            }
            return qbResponse;
        }
        private HttpClient InitializeHttpClient(IHttpClientFactory httpClientFactory, string accessToken)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            string apiBaseUrl = Convert.ToString(Config["QuickBooksOAuth:ApiBaseUrl"]);
            httpClient.BaseAddress = new Uri(apiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return httpClient;

        }

        //Method For the Quickbook desktop user
        public async Task<(HashSet<string>, UserInfo)> AuthenticateQBDesktopUser(string email, string password)
        {
            var message = new HashSet<string>();

            if (string.IsNullOrEmpty(email))
            {
                message.Add("Enter Email");
                return (message, null);
            }

            Vuser user = await Uow.Repository<Vuser>().SingleOrDefaultAsync(t => t.Email.ToLower() == email.ToLower() && t.Active == true);
            if (user == null)
            {
                message.Add("The email is not registered with the BMK Community");
                return (message, null);
            }
            if (!(bool)user.IsO365user && string.IsNullOrEmpty(password))
            {
                message.Add("Enter Password");
                return (message, null);
            }
            if (!(bool)user.IsO365user && !PasswordHash.VerifySignature(password, user.Credential, user.Salt))
            {
                message.Add("The password entered is incorrect");
                return (message, null);
            }
            UserInfo userInfo = new UserInfo
            {
                UserId = user.UsersId,
                RoleId = Convert.ToInt32(user.RoleMasterId),
                Email = user.Email,
                OrganizationId = Convert.ToInt32(user.OrganizationId),
            };
            SessionProvider.SetObject("user_info_copy", userInfo);
            return (message, userInfo);
        }
        public async Task QBDesktopGetAccountList(UserInfo userInfo, string accountInfoXML)
        {
            if (userInfo == null)
            {
                await QbExceptionLog("UserInfo Not Found In Session", "QuickBookDomain.QBDesktopGetAccountList", "Error occur Dued to UserInfo Not Found Session", userInfo);
            }
            else
            {
                try
                {
                    var spParameters = new SqlParameter[4];
                    spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = userInfo.OrganizationId };
                    spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = userInfo.UserId };
                    spParameters[2] = new SqlParameter() { ParameterName = "accountListXML", Value = accountInfoXML };
                    spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBD" };
                    var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountListUpdateV2", spParameters);
                    
                }
                catch (Exception ex)
                {
                    await QbExceptionLog(ex.Message, "QuickBookDomain.QBDesktopGetAccountList", "Error occur while retrieve accounts from Quickbook desktop ", userInfo);
                }
            }
        }

        public async Task QBDesktopGetAccountBalanceSheetListMonthWise(UserInfo userInfo, string accountBalanceXML)
        {
            if (userInfo == null)
            {
                await QbExceptionLog("UserInfo Not Found In Session", "QuickBookDomain.QBDesktopGetAccountBalanceSheetListMonthWise", "Error occur Dued to UserInfo Not Found Session", userInfo);
            }
            else
            {
                try
                {
                    var spParameters = new SqlParameter[4];
                    spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = userInfo.OrganizationId };
                    spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = userInfo.UserId };
                    spParameters[2] = new SqlParameter() { ParameterName = "accountBalanceXML", Value = accountBalanceXML };
                    spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBD" };
                    var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountUpdateBSV2", spParameters);
                }
                catch (Exception ex)
                {
                    await QbExceptionLog(ex.Message, "QuickBookDomain.QBDesktopGetAccountBalanceSheetListMonthWise", "Error occur while retrieve accounts BalanceSheet from Quickbook desktop ", userInfo);
                }
            }
        }

        public async Task QBDesktopGetAccountProfitAndLossListMonthWise(UserInfo userInfo, string accountBalanceXML)
        {
            if (userInfo == null)
            {
                await QbExceptionLog("UserInfo Not Found In Session", "QuickBookDomain.QBDesktopGetAccountBalanceSheetListMonthWise", "Error occur Dued to UserInfo Not Found Session", userInfo);
            }
            else
            {
                try
                {
                    var spParameters = new SqlParameter[4];
                    spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = userInfo.OrganizationId };
                    spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = userInfo.UserId };
                    spParameters[2] = new SqlParameter() { ParameterName = "accountBalanceXML", Value = accountBalanceXML };
                    spParameters[3] = new SqlParameter() { ParameterName = "qbPlatform", Value = "QBD" };
                    var result = await DbContextManager.StoreProc<object>("[dbo].spQBAccountUpdatePLV2", spParameters);
                }
                catch (Exception ex)
                {
                    await QbExceptionLog(ex.Message, "QuickBookDomain.QBDesktopGetAccountProfitAndLossListMonthWise", "Error occur while retrieve accounts P&L from Quickbook desktop ", userInfo);
                }
            }
        }
        //Comman method which used for both user
        public List<(string start, string end, string monthYear)> GetBalanceSheetDateList()
        {
            DateTime currentDate = DateTime.Now;
            int startYear = currentDate.Year - 5;
            int endYear = currentDate.Year;

            List<(string start, string end, string monthYear)> allDates = allDates = new List<(string start, string end, string monthYear)>();

            for (int year = startYear; year <= endYear; year++)
            {
                int startMonth = (year == startYear) ? currentDate.Month : 1;
                int endMonth = (year == endYear) ? currentDate.Month : 12; //if year is current year then last month will be previous month of current month

                for (int month = startMonth; month <= endMonth; month++)
                {
                    string startDate = $"{year}-{month:D2}-01";
                    string endDate = $"{year}-{month:D2}-{DateTime.DaysInMonth(year, month)}";
                    string monthYear = $"{month:D2}-{year}";

                    allDates.Add((startDate, endDate, monthYear));
                }
            }
            return allDates;
        }
        public async Task QbExceptionLog(string errorMessage, string methdoName, string url, UserInfo userInfo)
        {
            if (userInfo == null)
                userInfo = new UserInfo { UserId = 10000, OrganizationId = 10000 };

            QbExceptionLog log = new QbExceptionLog
            {
                ErrorMessage = errorMessage,
                ErrorProcedure = methdoName,
                OrganizationId = userInfo.OrganizationId,
                UsersId = userInfo.UserId,
                ErrorDate = DateTime.Now,
                JsonString = url,
            };
            await Uow.RegisterNewAsync<QbExceptionLog>(log);
            await Uow.CommitAsync();
        }
        public async Task AddQBProccesLog(string message, string responseStream = null)
        {
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            QbProcessLog log = new QbProcessLog
            {
                LogComments = message,
                OrganizationId = userInfo.OrganizationId,
                Active = true,
                ResponseStream = responseStream,
                LogDate = DateTime.Now
            };
            await Uow.RegisterNewAsync<QbProcessLog>(log);
            await Uow.CommitAsync();
        }

        public async Task<Response<object>> DeleteAllFinancialData(string deleteType)
        {
            Response<object> response = new Response<object>();
            try
            {
                var spParameters = new SqlParameter[3];
                spParameters[0] = new SqlParameter() { ParameterName = "organizationID", Value = UserClaim.OrganizationId };
                spParameters[1] = new SqlParameter() { ParameterName = "userID", Value = UserClaim.UserId };
                spParameters[2] = new SqlParameter() { ParameterName = "type", Value = deleteType };
                await DbContextManager.StoreProc<object>("[dbo].spQBDeleteAllData", spParameters);
                response.IsSucceed = true;
                response.Message = "All financial data has been deleted successfully";
            }
            catch (Exception ex)
            {
                await LogException.Log(ex, "api/QuickBooks/deleteAllFinancialData");
                response.IsSucceed = false;
                response.Message = "Error occur during deleting all financial data";
            }
            return response;
        }

        private async Task setQuickBoookSession()
        {
            UserInfo userInfo = SessionProvider.GetObject<UserInfo>("user_info");
            if (userInfo == null)
            {
                Vuser user = await Uow.Repository<Vuser>().SingleOrDefaultAsync(u => u.UsersId == UserClaim.UserId);
                if (user != null)
                {
                    UserInfo userSession = new UserInfo
                    {
                        UserId = user.UsersId,
                        RoleId = Convert.ToInt32(user.RoleMasterId),
                        Email = user.Email,
                        OrganizationId = Convert.ToInt32(user.OrganizationId),
                    };
                    SessionProvider.SetObject("user_info", userSession);
                }
            }
        }

    }
    public interface IQuickBookDomain
    {
        //Method For the Quickbook Online user
        Task<string> Connect();
        Task<string> CheckQbStatus();
        Task<Response<object>> Callback(string code = "", string state = "", string customerId = "", string url = "", bool isRefreshToken = false, string refreshToken = "");
        Task<Response<object>> RetrieveQbData();
        Task<Response<object>> GetAccountListAsync(long customerID, string accessToken);
        Task<Response<object>> GetMonthWiseBalanceSheetDataAsync(long customerId, string startDate, string endDate, string accesstoken);
        Task<Response<object>> GetRefershTokenAsync(QbTokenDetail qbDetails);

        //Method For the Quickbook desktop user
        Task<(HashSet<string>, UserInfo)> AuthenticateQBDesktopUser(string email, string password);
        Task QBDesktopGetAccountList(UserInfo userInfo, string accountInfoXML);
        Task QBDesktopGetAccountBalanceSheetListMonthWise(UserInfo userInfo, string accountBalanceXML);
        Task QBDesktopGetAccountProfitAndLossListMonthWise(UserInfo userInfo, string accountBalanceXML);
        //Comman method which used for both desktop and online user
        List<(string start, string end, string monthYear)> GetBalanceSheetDateList();
        Task QbExceptionLog(string errorMessage, string methdoName, string url, UserInfo userInfo);
        Task AddQBProccesLog(string message, string responseStream = "");
        Task<Response<object>> DeleteAllFinancialData(string deleteType);

    }
}
