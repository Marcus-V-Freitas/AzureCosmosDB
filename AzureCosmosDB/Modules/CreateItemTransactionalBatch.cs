using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class CreateItemTransactionalBatch : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            string partitionKeyName = "Canada";

            var volcanos = cosmosConfigs.Volcanos.Where(x => x.Country == partitionKeyName).Take(2).ToList();

            PartitionKey partitionKey = new(partitionKeyName);

            TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey)
                                                .CreateItem(volcanos[0])
                                                .CreateItem(volcanos[1]);


            using (var response = await batch.ExecuteAsync())
            {
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Request Charge: {response.RequestCharge}");
                Console.WriteLine($"Volcano 1: {response.GetOperationResultAtIndex<Volcano>(0).Resource.id}");
                Console.WriteLine($"Volcano 2: {response.GetOperationResultAtIndex<Volcano>(1).Resource.id}");
            }
        }
    }
}
