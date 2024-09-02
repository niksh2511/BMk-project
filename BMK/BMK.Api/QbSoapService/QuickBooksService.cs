using BMK.Domain.Domain;
using BMK.Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using System.Xml;

namespace BMK.Api.Services
{
    public class QuickBooksService : IQuickBooksService
    {
        public int count = 0;
        public ArrayList req = new ArrayList(); //save the all the quickbook request
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IQuickBookDomain QuickBookDomain { get; set; }

        public QuickBooksService(IHttpContextAccessor httpContextAccessor, IQuickBookDomain quickBookDomain)
        {
            HttpContextAccessor = httpContextAccessor;
            QuickBookDomain = quickBookDomain;
            req = buildRequest();
        }
        public string serverVersion()
        {
            // Return the version of your SOAP service
            return "1.0";
        }

        public async Task<string> clientVersion(string strVersion)
        {
            const double recommendedVersion = 1.5;
            const double supportedMinVersion = 1.0;

            double suppliedVersion = Convert.ToDouble(parseForVersion(strVersion));

            string retVal = null;
            if (suppliedVersion < supportedMinVersion)
            {
                retVal = "E:You need to upgrade your QBWebConnector";
            }
            else if (suppliedVersion < recommendedVersion)
            {
                retVal = "W:We recommend that you upgrade your QBWebConnector";
            }
            string evLogTxt = $"WebMethod: clientVersion() has been called by QBWebconnector" + "\r\n\r\n"
                                + "Parameters received:\r\n"
                                + "string strVersion = " + strVersion + "\r\n"
                                + "QBWebConnector version = " + strVersion + "\r\n"
                                + "Recommended Version = " + recommendedVersion.ToString() + "\r\n"
                                + "Supported Minimum Version = " + supportedMinVersion.ToString() + "\r\n"
                                + "SuppliedVersion = " + suppliedVersion.ToString() + "\r\n"
                                + "Return values: " + "\r\n"
                                + "string retVal = " + retVal;
            await QuickBookDomain.QbExceptionLog(evLogTxt, "QuickBooksService.clientVersion", "version mismatch", getUserInfo());
            return retVal;
        }

        public async Task<string[]> authenticate(string userName, string password)
        {
            try
            {
                string evLogTxt = $"WebMethod: authenticate() has been called by QBWebconnector. Parameters received: string strUserName = {userName}, string strPassword = {password}";
                string[] authenticateGuid = new string[2];
                var (message, userInfo) = await QuickBookDomain.AuthenticateQBDesktopUser(userName, password);
                if (message.Count() > 0 && userInfo == null)
                {
                    return message.ToArray();
                }
                else
                {
                    var serializedValue = System.Text.Json.JsonSerializer.Serialize(userInfo);
                    HttpContextAccessor.HttpContext.Session.SetString("user_info", serializedValue);
                    authenticateGuid[0] = Convert.ToString(Guid.NewGuid());
                    await QuickBookDomain.AddQBProccesLog("Web Connector Authenticated!");
                    //    req = buildRequest();
                    return authenticateGuid;
                }
            }
            catch (Exception ex)
            {
                await QuickBookDomain.QbExceptionLog(ex.Message, "QuickBooksService.authenticate", $"Faild Desktop Authenticate for {userName}", null);
                return new string[2] { ex.Message, "" };
            }
        }

        private string parseForVersion(string input)
        {
            // This method is created just to parse the first two version components
            // out of the standard four component version number:
            // <Major>.<Minor>.<Release>.<Build>
            // 
            // As long as you get the version in right format, you could use
            // any algorithm here. 
            string retVal = "";
            string major = "";
            string minor = "";
            Regex version = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)(\.\w+){0,2}$", RegexOptions.Compiled);
            Match versionMatch = version.Match(input);
            if (versionMatch.Success)
            {
                major = versionMatch.Result("${major}");
                minor = versionMatch.Result("${minor}");
                retVal = major + "." + minor;
            }
            else
            {
                retVal = input;
            }
            return retVal;
        }

        public async Task<string> sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName,
            string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers)
        {
            if (HttpContextAccessor.HttpContext.Session.GetInt32("counter") == null)
            {
                HttpContextAccessor.HttpContext.Session.SetInt32("counter", 0);
            }

            //ArrayList req = buildRequest();
            string request = "";
            int total = req.Count;
            count = Convert.ToInt32(HttpContextAccessor.HttpContext.Session.GetInt32("counter"));

            if (count < total)
            {
                request = Convert.ToString(req[count]);

                HttpContextAccessor.HttpContext.Session.SetInt32("counter", count + 1);
            }
            else
            {
                count = 0;
                HttpContextAccessor.HttpContext.Session.SetInt32("counter", 0);
                request = "";
            }
            await QuickBookDomain.AddQBProccesLog($"count:{HttpContextAccessor.HttpContext.Session.GetInt32("counter")}", request);

            return request;
        }

        public async Task<int> receiveResponseXML(string ticket, string response, string hresult, string message)
        {
            int count = Convert.ToInt32(HttpContextAccessor.HttpContext.Session.GetInt32("counter"));
            string evLogTxt = "WebMethod: receiveResponseXML() has been called by QBWebconnector" + "\r\n\r\n";
            evLogTxt = evLogTxt + "Parameters received:\r\n";
            evLogTxt = evLogTxt + "string ticket = " + ticket + "\r\n";
            evLogTxt = evLogTxt + "string response = " + response + "\r\n";
            evLogTxt = evLogTxt + "string hresult = " + hresult + "\r\n";
            evLogTxt = evLogTxt + "string message = " + message + "\r\n";
            evLogTxt = evLogTxt + "\r\n";

            int retVal = 0;
            await QuickBookDomain.AddQBProccesLog($"{ticket}{hresult}{message}", response);
            if (!hresult.ToString().Equals(""))
            {
                // if there is an error with response received, web service could also return a -ve int		
                evLogTxt = evLogTxt + "HRESULT = " + hresult + "\r\n";
                evLogTxt = evLogTxt + "Message = " + message + "\r\n";
                retVal = -101;
            }
            else
            {
                UserInfo? userInfo = getUserInfo();
                if (response != null)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(response);
                    XmlNode accountQueryRsNode = xmlDocument.SelectSingleNode("//AccountQueryRs");
                    XmlNode accountBalanceSheetNode = xmlDocument.SelectSingleNode("//GeneralSummaryReportQueryRs ");

                    if (accountQueryRsNode != null && accountQueryRsNode.Attributes["requestID"] != null)
                    {
                        //based on the buildRequest method
                        //requestID = 1 // receive accountlist data
                        //requestID >= 1 // receive account balance sheet data
                        string requestID = accountQueryRsNode.Attributes["requestID"].Value;
                        if (userInfo != null)
                        {
                            if (requestID == "1")
                                await QuickBookDomain.QBDesktopGetAccountList(userInfo, response);
                        }
                        else
                        {
                            await QuickBookDomain.QbExceptionLog("User info not found in Session", Convert.ToString(count), "Quick book Desktop Error", userInfo);
                        }
                    }
                    else if (accountBalanceSheetNode != null && accountBalanceSheetNode.Attributes["requestID"] != null)
                    {
                        string requestID = accountBalanceSheetNode.Attributes["requestID"].Value;
                        if (userInfo != null)
                        {
                            if (requestID != "1")
                            {
                                if(Convert.ToInt32(requestID) % 2 == 0)
                                {
                                    await QuickBookDomain.QBDesktopGetAccountBalanceSheetListMonthWise(userInfo, response);
                                }
                                else
                                {
                                    await QuickBookDomain.QBDesktopGetAccountProfitAndLossListMonthWise(userInfo, response);
                                }
                            }
                               
                        }
                        else
                        {
                            await QuickBookDomain.QbExceptionLog("User info not found in Session", Convert.ToString(count), "Quick book Desktop Error", userInfo);
                        }
                    }
                    else
                    {
                        await QuickBookDomain.QbExceptionLog("User info not found in Session", Convert.ToString(count), "Quick book Desktop Error", userInfo);
                    }
                }
                else
                {
                    await QuickBookDomain.QbExceptionLog("response was not found when fetching quickbook desktop data", Convert.ToString(count), "Response not found", userInfo);
                }
            }

            evLogTxt = evLogTxt + "Length of response received = " + response.Length + "\r\n";

            //ArrayList req = buildRequest();
            int total = req.Count;


            int percentage = (count * 100) / total;
            if (percentage >= 100)
            {
                count = 0;
                HttpContextAccessor.HttpContext.Session.SetInt32("counter", 0);
            }
            retVal = percentage;

            evLogTxt = evLogTxt + "\r\n";
            evLogTxt = evLogTxt + "Return values: " + "\r\n";
            evLogTxt = evLogTxt + "int retVal= " + retVal.ToString() + "\r\n";
            return retVal;
        }

        public async Task<string> closeConnection(string ticket)
        {
            await QuickBookDomain.AddQBProccesLog("Closing Connection");
            return "OK";
        }
        public async Task<string> connectionError(string ticket, string hresult, string message)
        {
            string retVal = "";
            string evLogTxt = $"WebMethod: connectionError() has been called by QBWebconnector \r\n\r\n" +
                     "Parameters received:\r\n" +
                     $"string ticket = {ticket}\r\n" +
                     $"string hresult = {hresult}\r\n" +
                     $"string message = {message}\r\n\r\n";

            var value = HttpContextAccessor.HttpContext.Session.GetString("user_info");
            var userInfo = value == null ? default : System.Text.Json.JsonSerializer.Deserialize<UserInfo>(value);
            await QuickBookDomain.AddQBProccesLog("Connection Error Occur");
            try
            {
                int ceCounter = HttpContextAccessor.HttpContext.Session.GetInt32("ce_counter") ?? 0;
                HttpContextAccessor.HttpContext.Session.SetInt32("ce_counter", ceCounter);

                // 0x80040400 - QuickBooks found an error when parsing the provided XML text stream. 
                const string QB_ERROR_WHEN_PARSING = "0x80040400";
                // 0x80040401 - Could not access QuickBooks.  
                const string QB_COULDNT_ACCESS_QB = "0x80040401";
                // 0x80040402 - Unexpected error. Check the qbsdklog.txt file for possible, additional information. 
                const string QB_UNEXPECTED_ERROR = "0x80040402";
                // Add more as you need...
                bool isKnownError = false;
                string errorType = "";

                switch (hresult.Trim())
                {
                    case QB_ERROR_WHEN_PARSING:
                        isKnownError = true;
                        errorType = "QuickBooks found an error when parsing the provided XML text stream.";
                        break;
                    case QB_COULDNT_ACCESS_QB:
                        isKnownError = true;
                        errorType = "Could not access QuickBooks.";
                        break;
                    case QB_UNEXPECTED_ERROR:
                        isKnownError = true;
                        errorType = "Unexpected error. Check the qbsdklog.txt file for possible, additional information.";
                        break;
                }


                if (isKnownError)
                {
                    evLogTxt += $"Known error occurred: {errorType}\r\n";
                    retVal = "DONE";
                    evLogTxt += "Sending DONE to stop.\r\n";
                }
                else if (ceCounter == 0)
                {
                    retVal = "";
                    evLogTxt += "Sending empty company file to try again.\r\n";
                    errorType = "unkkown error but Sending empty company file to try again";
                }
                else
                {
                    retVal = "DONE";
                    evLogTxt += "Sending DONE to stop.\r\n";
                    errorType = "unkkown error but Sending DONE to stop.";
                }
                evLogTxt += $"Return values:\r\nstring retVal = {retVal}\r\n";
                await QuickBookDomain.QbExceptionLog(evLogTxt, "QuickBooksService.connectionError", errorType, userInfo);
                HttpContextAccessor.HttpContext.Session.SetInt32("ce_counter", ceCounter + 1);

            }
            catch (Exception ex)
            {
                await QuickBookDomain.QbExceptionLog(ex.Message, "QuickBooksService.connectionError", "Connection Error while fetching quickbook desktop data", userInfo);
                retVal = "";
            }
            return retVal;
        }
        public async Task<string> getLastError(string ticket)
        {
            string evLogTxt = $"WebMethod: getLastError() has been called by QBWebconnector\r\n\r\n" +
                      "Parameters received:\r\n" +
                      $"string ticket = {ticket}\r\n\r\n";


            int errorCode = 0;
            string retVal = "";
            if (errorCode == -101)
            {
                retVal = "QuickBooks was not running!"; // This is just an example of custom user errors
            }
            else
            {
                retVal = "Error!";
            }
            evLogTxt += $"Return values:\r\nstring retVal = {retVal}\r\n";
            var value = HttpContextAccessor.HttpContext.Session.GetString("user_info");
            var userInfo = value == null ? default : System.Text.Json.JsonSerializer.Deserialize<UserInfo>(value);
            await QuickBookDomain.QbExceptionLog(evLogTxt, "QuickBooksService.getLastError", "", userInfo);
            return retVal;
        }

        public ArrayList buildRequest()
        {
            string strRequestXML = "";

            #region ACCOUNTLIST
            // AccountQuery
            XmlDocument xmlAccountList = new XmlDocument();
            xmlAccountList.AppendChild(xmlAccountList.CreateXmlDeclaration("1.0", null, null));
            xmlAccountList.AppendChild(xmlAccountList.CreateProcessingInstruction("qbxml", "version=\"16.0\""));

            XmlElement qbXML = xmlAccountList.CreateElement("QBXML");
            xmlAccountList.AppendChild(qbXML);
            XmlElement qbXMLMsgsRq = xmlAccountList.CreateElement("QBXMLMsgsRq");
            qbXML.AppendChild(qbXMLMsgsRq);
            qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
            XmlElement customerQueryRq = xmlAccountList.CreateElement("AccountQueryRq");
            qbXMLMsgsRq.AppendChild(customerQueryRq);
            customerQueryRq.SetAttribute("requestID", "1");
            string[] fields = new string[] { "Name", "FullName",
                "IsActive",  "AccountType", "SpecialAccountType", "AccountNumber",
                "BankNumber", "Desc", "OpenBalance", "TotalBalance" };
            foreach (var field in fields)
            {
                XmlElement includeRetElement = xmlAccountList.CreateElement(string.Empty, "IncludeRetElement", string.Empty);
                includeRetElement.InnerText = field;
                customerQueryRq.AppendChild(includeRetElement);
            }
            //customerQueryRq.SetAttribute("MethodName", "GetAccountList");
            
            strRequestXML = xmlAccountList.OuterXml;
            req.Add(strRequestXML);
            #endregion
            // Clean up
            strRequestXML = "";
           
            //get last five year startdate and enddate month with month-year
            List<(string start, string end, string monthYear)> dateRanges = QuickBookDomain.GetBalanceSheetDateList();
            int step = 1;
            foreach (var dateRange in dateRanges)
            {
                step = Convert.ToInt32(step) + 1;
                #region BALANCESHEET
                // TrailBalanceSheet
                // Create an XML document
                XmlDocument xmlBalanceSheet = new XmlDocument();

                // Create XML declaration
                XmlDeclaration xmlDeclaration = xmlBalanceSheet.CreateXmlDeclaration("1.0", null, null);

                xmlBalanceSheet.AppendChild(xmlDeclaration);
                xmlBalanceSheet.AppendChild(xmlBalanceSheet.CreateProcessingInstruction("qbxml", "version=\"16.0\""));
                // Create QBXML element
                XmlElement qbxmlElement = xmlBalanceSheet.CreateElement("QBXML");
                xmlBalanceSheet.AppendChild(qbxmlElement);

                // Create QBXMLMsgsRq element with onError attribute
                XmlElement qbxmlMsgsRqElement = xmlBalanceSheet.CreateElement("QBXMLMsgsRq");
                qbxmlMsgsRqElement.SetAttribute("onError", "stopOnError");
                qbxmlElement.AppendChild(qbxmlMsgsRqElement);

                // Create GeneralSummaryReportQueryRq element
                XmlElement generalSummaryReportQueryRqElement = xmlBalanceSheet.CreateElement("GeneralSummaryReportQueryRq");
                generalSummaryReportQueryRqElement.SetAttribute("requestID", step.ToString());
                qbxmlMsgsRqElement.AppendChild(generalSummaryReportQueryRqElement);

                // Create GeneralSummaryReportType element
                XmlElement generalSummaryReportTypeElement = xmlBalanceSheet.CreateElement("GeneralSummaryReportType");
                generalSummaryReportTypeElement.InnerText = "BalanceSheetStandard"; //"BalanceSheetStandard";
                generalSummaryReportQueryRqElement.AppendChild(generalSummaryReportTypeElement);

                // Create DisplayReport element
                XmlElement displayReportElement = xmlBalanceSheet.CreateElement("DisplayReport");
                displayReportElement.InnerText = "false";
                generalSummaryReportQueryRqElement.AppendChild(displayReportElement);

                // Create ReportPeriod element
                XmlElement reportPeriodElement = xmlBalanceSheet.CreateElement("ReportPeriod");
                generalSummaryReportQueryRqElement.AppendChild(reportPeriodElement);

                // Create FromReportDate element
                XmlElement fromReportDateElement = xmlBalanceSheet.CreateElement("FromReportDate");
                fromReportDateElement.InnerText = dateRange.start;
                reportPeriodElement.AppendChild(fromReportDateElement);

                // Create ToReportDate element
                XmlElement toReportDateElement = xmlBalanceSheet.CreateElement("ToReportDate");
                toReportDateElement.InnerText = dateRange.end;
                reportPeriodElement.AppendChild(toReportDateElement);
                strRequestXML = xmlBalanceSheet.OuterXml;
                req.Add(strRequestXML);
                #endregion

                step = Convert.ToInt32(step) + 1;
                #region PROFITANDLOSS
                // TrailBalanceSheet
                // Create an XML document
                XmlDocument xmlProfitAndLoss = new XmlDocument();

                // Create XML declaration
                XmlDeclaration xmlPFDeclaration = xmlProfitAndLoss.CreateXmlDeclaration("1.0", null, null);

                xmlProfitAndLoss.AppendChild(xmlPFDeclaration);
                xmlProfitAndLoss.AppendChild(xmlProfitAndLoss.CreateProcessingInstruction("qbxml", "version=\"16.0\""));
                // Create QBXML element
                XmlElement qbFLxmlElement = xmlProfitAndLoss.CreateElement("QBXML");
                xmlProfitAndLoss.AppendChild(qbFLxmlElement);

                // Create QBXMLMsgsRq element with onError attribute
                XmlElement qbFLxmlMsgsRqElement = xmlProfitAndLoss.CreateElement("QBXMLMsgsRq");
                qbFLxmlMsgsRqElement.SetAttribute("onError", "stopOnError");
                qbFLxmlElement.AppendChild(qbFLxmlMsgsRqElement);

                // Create GeneralSummaryReportQueryRq element
                XmlElement generalSummaryReportFLQueryRqElement = xmlProfitAndLoss.CreateElement("GeneralSummaryReportQueryRq");
                generalSummaryReportFLQueryRqElement.SetAttribute("requestID", step.ToString());
                qbFLxmlMsgsRqElement.AppendChild(generalSummaryReportFLQueryRqElement);

                // Create GeneralSummaryReportType element
                XmlElement generalSummaryReportFLTypeElement = xmlProfitAndLoss.CreateElement("GeneralSummaryReportType");
                generalSummaryReportFLTypeElement.InnerText = "ProfitAndLossStandard"; //"BalanceSheetStandard";
                generalSummaryReportFLQueryRqElement.AppendChild(generalSummaryReportFLTypeElement);

                // Create DisplayReport element
                XmlElement displayReportFLElement = xmlProfitAndLoss.CreateElement("DisplayReport");
                displayReportFLElement.InnerText = "false";
                generalSummaryReportFLQueryRqElement.AppendChild(displayReportFLElement);

                // Create ReportPeriod element
                XmlElement reportPeriodFLElement = xmlProfitAndLoss.CreateElement("ReportPeriod");
                generalSummaryReportFLQueryRqElement.AppendChild(reportPeriodFLElement);

                // Create FromReportDate element
                XmlElement fromReportDateFLElement = xmlProfitAndLoss.CreateElement("FromReportDate");
                fromReportDateFLElement.InnerText = dateRange.start;
                reportPeriodFLElement.AppendChild(fromReportDateFLElement);

                // Create ToReportDate element
                XmlElement toReportDateFLElement = xmlProfitAndLoss.CreateElement("ToReportDate");
                toReportDateFLElement.InnerText = dateRange.end;
                reportPeriodFLElement.AppendChild(toReportDateFLElement);
                strRequestXML = xmlProfitAndLoss.OuterXml;
                req.Add(strRequestXML);
                #endregion

            }

            return req;
        }

        private UserInfo? getUserInfo()
        {
            var value = HttpContextAccessor.HttpContext.Session.GetString("user_info");
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<UserInfo>(value);
        }
    }
}
