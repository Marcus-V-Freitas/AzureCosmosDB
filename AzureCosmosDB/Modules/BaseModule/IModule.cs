using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB.Modules.BaseModule
{
    public interface IModule
    {
        public Task Run(CosmosConfigs cosmosConfigs);
    }
}
