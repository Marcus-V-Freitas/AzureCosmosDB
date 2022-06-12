using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class IndexPolicy : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            Database database = cosmosClient.GetDatabase(cosmosConfigs.Database);

            IndexingPolicy policy = new IndexingPolicy()
            {
                IndexingMode = IndexingMode.Consistent,
                Automatic = true
            };

            policy.ExcludedPaths.Add(
                new() { Path = "/*" }
                );

            policy.IncludedPaths.Add(
                new() { Path = "/Status/?" }
                );            

            var compositeRegion = new CompositePath()
            {
                Path = "/Region",
                Order = CompositePathSortOrder.Ascending
            };

            var compositeElevation = new CompositePath()
            {
                Path = "/Elevation",
                Order = CompositePathSortOrder.Descending
            };

            Collection<CompositePath> compositePaths = new()
            {
                compositeRegion,
                compositeElevation
            };

            policy.CompositeIndexes.Add(compositePaths);

            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = "Test",
                PartitionKeyPath = cosmosConfigs.PartitionKey,
                IndexingPolicy = policy,
            };

            await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties: ThroughputProperties.CreateManualThroughput(400));
        }
    }
}
