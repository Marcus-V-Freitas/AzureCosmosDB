using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class OptimisticConcurrencyControl : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {

            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            string partitionKeyName = "Canada";
            PartitionKey partitionKey = new(partitionKeyName);

            var volcanoResponse = await container.ReadItemAsync<Volcano>("757d8809-7db6-03bb-6d9d-18501baacb58", partitionKey);
            
            var volcano = volcanoResponse.Resource;
            string etag = volcanoResponse.ETag;

            volcano.Region = volcano.Region.ToUpper();

            ItemRequestOptions itemRequest = new()
            {
                IfMatchEtag = etag,
            };

            volcanoResponse = await container.UpsertItemAsync<Volcano>(volcano, partitionKey, itemRequest);

            Console.WriteLine($"Id: {volcanoResponse.Resource.id}");
            Console.WriteLine($"Request Charge: {volcanoResponse.RequestCharge}");
            Console.WriteLine($"Etag: {volcanoResponse.ETag}");
            Console.WriteLine($"Status Code: {volcanoResponse.StatusCode}");
        }
    }
}
