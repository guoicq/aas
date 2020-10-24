using System;
using System.Data;
using System.Threading.Tasks;

namespace AASSample
{
    class Program
    {
        static void Main(string[] args)
        {
            QueryProduct().Wait();
        }

        static async Task QueryProduct()
        {
            var setting = new AASClientSetting
            {
                Region = "AAS Region",
                ServerName = "AAS Server Name",
                DatabaseName = "AAS Database Name",
                TenantId = "App Principle Tenant Id",
                ClientId = "App Priciple Client Id",
                ClientSecret = "App Priciple Client Secret",
            };

            var client = new AASClient(setting);
            
            var results = client.ExecuteDaxQueryAsync("EVALUATE('Product')");
            await foreach (var item in results)
            {
                Console.WriteLine($"{item[0]} {item[1]}");
            }
        }
    }
}
