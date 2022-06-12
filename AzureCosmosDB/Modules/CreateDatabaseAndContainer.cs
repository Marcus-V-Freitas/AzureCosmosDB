using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules
{
    public class CreateDatabaseAndContainer : IModule
    {
        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosConfigs.Database);

            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new()
            {
                Id = cosmosConfigs.Container,
                PartitionKeyPath = cosmosConfigs.PartitionKey
            }, ThroughputProperties.CreateManualThroughput(400));

        }
    }
}
