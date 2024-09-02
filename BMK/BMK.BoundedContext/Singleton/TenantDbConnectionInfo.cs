using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RxWeb.Core.Data;
using RxWeb.Core.Data.Models;
using System.Collections.Concurrent;

namespace BMK.BoundedContext.Singleton
{
    public class TenantDbConnectionInfo : ITenantDbConnectionInfo
    {

        public TenantDbConnectionInfo(IOptions<DatabaseConfig> databaseConfig)
        {
            ConnectionInfo = new ConcurrentDictionary<string, Dictionary<string, string>>();
            DatabaseConfig = databaseConfig.Value;
        }
        private ConcurrentDictionary<string, Dictionary<string, string>> ConnectionInfo { get; set; }

        public async Task<Dictionary<string, string>> GetAsync(string hostUri)
        {
            var connectionInfo = GetConnectionString(hostUri);
            if (connectionInfo == null)
            {
                return await SetUpDataAndGetConnectionInfo(hostUri);
            }
            return null;
        }

        private Dictionary<string, string> GetConnectionString(string hostUri)
        {
            Dictionary<string, string> cacheValue;
            if (ConnectionInfo.TryGetValue(hostUri, out cacheValue))
            {
                return cacheValue;
            }
            return null;
        }


        private async Task<Dictionary<string, string>> SetUpDataAndGetConnectionInfo(string uri)
        {
            var sqlConnection = new SqlConnection(DatabaseConfig.ConnectionString["##ConnectionName##"]);
            try
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("select ##0##, ##1##,##2##, ##3## from ##4##", sqlConnection);
                var dataReader = sqlCommand.ExecuteReader();
                var hostUri = string.Empty;
                var connectionInfo = new Dictionary<string, string>();
                while (dataReader.Read())
                {
                    var currentHostUri = dataReader.GetString(0);
                    if (hostUri != string.Empty && hostUri != currentHostUri)
                    {
                        Save(hostUri, connectionInfo);
                        connectionInfo = new Dictionary<string, string>();
                    }
                    connectionInfo.Add(dataReader.GetString(1), dataReader.GetString(2));
                    hostUri = currentHostUri;
                }
                Save(hostUri, connectionInfo);
                dataReader.Close();
                await dataReader.DisposeAsync();
                await sqlConnection.CloseAsync();
            }
            finally
            {
                await sqlConnection.DisposeAsync();
            }
            return GetConnectionString(uri);
        }

        public void Save(string hostUri, Dictionary<string, string> value)
        {
            ConnectionInfo.AddOrUpdate(hostUri, value, (x, y) => value);
        }

        private DatabaseConfig DatabaseConfig { get; set; }
    }
}

