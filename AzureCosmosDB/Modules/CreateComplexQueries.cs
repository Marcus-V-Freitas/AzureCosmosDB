using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class CreateComplexQueries : IModule
    {
        public class WrapperVolcano
        {
            public string Volcano { get; set; }
            public string Coordinate { get; set; }
        }

        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);

            var query = @"SELECT 
                          c['Volcano Name'] as Volcano,
                          v as Coordinate
                          FROM c
                          JOIN
                          (SELECT VALUE t FROM t IN c.Location.coordinates WHERE t > @value) as v";


            var queryDefinitions = new QueryDefinition(query)
                .WithParameter("@value", 170);


            QueryRequestOptions options = new()
            {
                MaxItemCount = 20
            };

            using (FeedIterator<WrapperVolcano> feedIterator = container.GetItemQueryIterator<WrapperVolcano>(queryDefinitions, null, options))
            {
                while (feedIterator.HasMoreResults)
                {
                    List<WrapperVolcano> wrappers = new();

                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        wrappers.Add(item);
                    }

                    Console.WriteLine($"Count:{wrappers.Count}");
                }
            }
        }
    }
}
