using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class MeasureIndexPerfomance : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);


            string query = @"SELECT * 
                             FROM c 
                             WHERE c.Elevation > @elevation
                             ORDER BY c.Region ASC, c.Elevation DESC";

            QueryDefinition queryDefinition = new QueryDefinition(query)
                                                    .WithParameter("@elevation", 100);

            QueryRequestOptions options = new QueryRequestOptions()
            {
                PopulateIndexMetrics = true,
                MaxItemCount         = 25, 
            };

            using (FeedIterator<Volcano> feedIterator = container.GetItemQueryIterator<Volcano>(queryDefinition, null, options))
            {
                double totalRUs = 0;
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<Volcano> response = await feedIterator.ReadNextAsync();

                    foreach (var item in response)
                    {

                    }

                    Console.WriteLine(response.IndexMetrics);
                    totalRUs += response.RequestCharge;
                }

                Console.WriteLine($"Total RUs: {totalRUs}");
            }
        }
    }
}
