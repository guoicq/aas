using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AASSample
{
    public class AASClient
    {
        private readonly AASClientSetting config;

        public AASClient(AASClientSetting config)
        {
            this.config = config;
        }

        public async IAsyncEnumerable<IDataRecord> ExecuteDaxQueryAsync(string query)
        {
            using (var connection = new AdomdConnection(this.GetConnectionString(await this.GetTokenAsync().ConfigureAwait(false))))
            //using (var connection = new AdomdConnection(this.GetConnectionString()))
            {
                connection.Open();
                using (var cmd = new AdomdCommand(query, connection))
                {
                    //cmd.Parameters.Add("BaseStation", "4999");
                    using (var reader = cmd.ExecuteReader())
                    {
                        foreach (var item in reader)
                            yield return item;
                    }
                }
            }
        }

        private string GetConnectionString()
        {
            var str = $"Provider = MSOLAP; Data Source = asazure://{config.Region}.asazure.windows.net/{config.ServerName};Initial Catalog={config.DatabaseName}";
            return str;
        }

        private string GetConnectionString(string token)
        {
            return $"Provider=MSOLAP;Data Source=asazure://{config.Region}.asazure.windows.net/{config.ServerName}:rw;Initial Catalog={config.DatabaseName};User ID=;Password={token};Persist Security Info=True;Impersonation Level=Impersonate";
        }

        private async Task<string> GetTokenAsync()
        {
            var resourceID = $"https://{config.Region}.asazure.windows.net";
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext($"https://login.windows.net/{config.TenantId}");
            ClientCredential credential = new ClientCredential(config.ClientId, config.ClientSecret);
            AuthenticationResult token = await authContext.AcquireTokenAsync(resourceID, credential).ConfigureAwait(false);
            return token.AccessToken; //Use this token to connect to AAS.
        }
    }
}
