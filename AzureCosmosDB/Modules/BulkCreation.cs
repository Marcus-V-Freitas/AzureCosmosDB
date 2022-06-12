using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class BulkCreation : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClientOptions options = new()
            {
                AllowBulkExecution = true,
            };

            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken, clientOptions: options);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            var volcanos = cosmosConfigs.Volcanos.Skip(2);

            List<Task<ItemResponse<Volcano>>> concurrentTaks = new();

            foreach (var volcano in volcanos)
            {
                concurrentTaks.Add(container.CreateItemAsync<Volcano>(volcano, new PartitionKey(volcano.Country)));
            }

            await Task.WhenAll(concurrentTaks);

            Console.WriteLine($"Completed Taks: {concurrentTaks.Where(x=>x.IsCompleted).Count()}");
            Console.WriteLine($"Request Charges: {concurrentTaks.Sum(x=> x.Result.RequestCharge)}");
        }
    }
}
