using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class CreateQueries : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            string query = @"SELECT DISTINCT
                             VALUE c['Volcano Name'] 
                             FROM c";


            QueryDefinition queryDefinition = new(query);

            using (FeedIterator<string> feedIterator = container.GetItemQueryIterator<string>(queryDefinition, null, new QueryRequestOptions() { }))
            {
                while (feedIterator.HasMoreResults)
                {
                    List<string> volcanosName = new();
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        volcanosName.Add(item);
                    }

                    Console.WriteLine($"Volcano Count: {volcanosName.Count}");
                }
            }
        }
    }
}
