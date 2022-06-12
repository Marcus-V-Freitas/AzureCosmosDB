using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Azure.Cosmos.Container;

namespace AzureCosmosDB.Modules
{
    public class ChangeFeed : IModule
    {
        private ChangesHandler<Volcano> changeHandlerDelegate = async (IReadOnlyCollection<Volcano> changes, 
                                                                       CancellationToken cancellationToken) => 
        {
            foreach (var change in changes)
            {
                await Console.Out.WriteLineAsync($"Detected Operation:\t[{change.id}]\t{change.VolcanoName}");
            }
        };

        private ChangesEstimationHandler changeEstimationDelegate = async (long estimation,
                                                                   CancellationToken cancellationToken) => 
        {
            await Console.Out.WriteLineAsync($"Estimation: {estimation}");
        };

        public async Task Run(CosmosConfigs cosmosConfigs)
        {
            CosmosClient cosmosClient = new CosmosClient(cosmosConfigs.AccountEndpoint, cosmosConfigs.AuthKeyOrResourceToken);
            var container = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.Container);
            var containerLease = cosmosClient.GetContainer(cosmosConfigs.Database, cosmosConfigs.ContainerLease);

            var processor = container.GetChangeFeedProcessorBuilder<Volcano>(processorName: "VolcanoItemProcessor", 
                                                                             onChangesDelegate: changeHandlerDelegate)
                                                                             .WithInstanceName("desktopAPP")
                                                                             .WithLeaseContainer(containerLease)
                                                                             .Build();

            var estimator = container.GetChangeFeedEstimatorBuilder(processorName: "VolcanoItemEstimator",
                                                                    estimationDelegate: changeEstimationDelegate)
                                                                    .WithLeaseContainer(containerLease)
                                                                    .Build();

            await estimator.StartAsync();
            await processor.StartAsync();




            await processor.StopAsync();
            await estimator.StopAsync();

        }
    }
}
