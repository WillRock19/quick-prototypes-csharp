using FileQueueCleanerConsoleApp.Models;

namespace FileQueueCleanerConsoleApp.Services
{
    internal class UserInputService
    {
        internal UserInputData GetInputData() 
        {
            Console.WriteLine("Cosmos db's connection string: ");
            var dbConnectionString = Console.ReadLine();

            Console.WriteLine("AzureStorage's connection string: ");
            var azureStorageConnectionString = Console.ReadLine();

            Console.WriteLine("Path to JSON file containing all records to be fixed: ");
            var pathToJsonFile = Console.ReadLine();

            return new()
            {
                AzureStorageConnectionString = azureStorageConnectionString ?? string.Empty,
                CosmosDbConnectionString = dbConnectionString ?? string.Empty,
                PathToJsonFileContainingFileQueuesToFix = pathToJsonFile ?? string.Empty,
            };
        }
    }
}
