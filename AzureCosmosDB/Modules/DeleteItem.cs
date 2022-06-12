using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class DeleteItem : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            PartitionKey partitionKey = new("Japan");

            var volcanoResponse = await container.DeleteItemAsync<Volcano>("4cb67ab0-ba1a-0e8a-8dfc-d48472fd5766", partitionKey);

            Console.WriteLine($"Request Charge: {volcanoResponse.RequestCharge}");
            Console.WriteLine($"Etag: {volcanoResponse.ETag}");
            Console.WriteLine($"Status Code: {volcanoResponse.StatusCode}");
        }
    }
}
