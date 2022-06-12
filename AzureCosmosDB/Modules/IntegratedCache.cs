using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class IntegratedCache : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClientOptions options = new()
            {
                ConnectionMode = ConnectionMode.Gateway
            };

            CosmosClient cosmosClient = new(cosmosConfigs.GatewayConnection, options);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            string id = "12e08a3a-155f-ac49-764a-e42403787ad1";
            PartitionKey partitionKey = new("Chile");

            ItemRequestOptions itemRequestOptions = new()
            {
                ConsistencyLevel = ConsistencyLevel.Session,                
            };

            ItemResponse<Volcano> response = await container.ReadItemAsync<Volcano>(id, partitionKey, itemRequestOptions);
            Console.WriteLine($"Total request charge:\t{response.RequestCharge:0.00} RU/s");

            response = await container.ReadItemAsync<Volcano>(id, partitionKey, itemRequestOptions);
            Console.WriteLine($"Total request charge:\t{response.RequestCharge:0.00} RU/s");

            QueryDefinition queryDefinition = new(@"SELECT TOP 10 * FROM c");

            QueryRequestOptions queryRequestOptions = new()
            {
                ConsistencyLevel = ConsistencyLevel.Eventual,
                MaxItemCount     = 5
            };

            using (FeedIterator<Volcano> iterator = container.GetItemQueryIterator<Volcano>(queryDefinition, null, queryRequestOptions))
            {
                double totalRequestCharge = 0;

                while (iterator.HasMoreResults)
                {                    
                    FeedResponse<Volcano> feedResponse = await iterator.ReadNextAsync();
                    foreach (var volcano in feedResponse)
                    {
                        totalRequestCharge += response.RequestCharge;
                        Console.WriteLine($"Total request charge:\t{totalRequestCharge:0.00} RU/s");
                    }                    
                }

                Console.WriteLine($"Total request charge:\t{totalRequestCharge:0.00} RU/s");
            }
        }
    }
}
