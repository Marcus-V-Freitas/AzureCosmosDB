using AzureCosmosDB.Modules.BaseModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB.Modules
{
    public class CreateItem : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            var volcano = cosmosConfigs.Volcanos.FirstOrDefault();

            PartitionKey partitionKey = new(volcano.Country);

            var volcanoResponse = await container.CreateItemAsync<Volcano>(volcano, partitionKey);

            Console.WriteLine($"Id: {volcanoResponse.Resource.id}");
            Console.WriteLine($"Request Charge: {volcanoResponse.RequestCharge}");
            Console.WriteLine($"Etag: {volcanoResponse.ETag}");
            Console.WriteLine($"Status Code: {volcanoResponse.StatusCode}");
        }
    }
}
