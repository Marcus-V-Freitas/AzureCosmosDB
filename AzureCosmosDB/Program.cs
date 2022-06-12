using AzureCosmosDB;
using AzureCosmosDB.Modules.BaseModule;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

// Load config object
var cosmosConfigs = config.GetSection("CosmosConfigs").Get<CosmosConfigs>();

// Get all modules
List<IModule> modules = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.GetInterfaces().Contains(typeof(IModule))
                         select Activator.CreateInstance(t) as IModule).ToList();


Dictionary<int, IModule> modulesKeys = new();

// Add index for easy access
for (int i = 1; i <= modules.Count(); i++)
{
    modulesKeys.Add(i, modules[i-1]);
}

do
{
    Console.WriteLine("Select the module by number:");
    Console.Clear();

    foreach (var item in modulesKeys)
    {
        Console.WriteLine($"{item.Key} - {item.Value.GetType().Name}");        
    }

    if (int.TryParse(Console.ReadLine(), out int key) && modulesKeys.TryGetValue(key, out IModule value))
    {
        await value.Run(cosmosConfigs);
    }

    Console.WriteLine("Do you wish to continue? Press 'N' to end");

} while(Console.ReadLine().ToUpper() != "N");

