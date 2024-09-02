using System.ServiceModel;

namespace BMK.Api.Services
{
    [ServiceContract(Namespace = "http://developer.intuit.com/")]
    public interface IQuickBooksService
    {
        [OperationContract]
        Task<string[]> authenticate(string strUserName, string strPassword);

        [OperationContract]
        Task<string> clientVersion(string strVersion);

        [OperationContract]
        string serverVersion();

        [OperationContract]
        Task<string> sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName,
         string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers);

        [OperationContract]
        Task<int> receiveResponseXML(string ticket, string response, string hresult, string message);

        [OperationContract]
        Task<string> connectionError(string ticket, string hresult, string message);

        [OperationContract]
        Task<string> closeConnection(string ticket);

        [OperationContract]
        Task<string> getLastError(string ticket);

    }
}
