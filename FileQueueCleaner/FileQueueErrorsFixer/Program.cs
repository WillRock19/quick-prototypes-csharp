using FileQueueErrorsFixer.Models;
using FileQueueErrorsFixer.Models.FileQueue;
using FileQueueErrorsFixer.Services;
using Microsoft.Extensions.Configuration;

namespace FileQueueErrorsFixer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = LoadConfigurations();

            var fixService = new FixRecordsService(configuration);

            var userInputData = GetValidUserInputs();

            var (fileContent, errorMessage) = new FileLoaderService()
                .LoadFileDataAsync(userInputData)
                .GetAwaiter()
                .GetResult();

            if (!string.IsNullOrEmpty(errorMessage) || fileContent is null)
            {
                Console.Error.WriteLine($"Could not load content from '{userInputData.PathToJsonFileContainingFileQueuesToFix}'.");
                Console.Error.WriteLine(errorMessage);
            }

            if (!fileContent!.All(x => x.isValid())) 
            {
                Console.Error.WriteLine($"One or more of the FileQueues on '{userInputData.PathToJsonFileContainingFileQueuesToFix}' does not contain a id or _etag properties.");
            }
            else
            {
                var resultsWithErrors = fixService.FixRecordsAsync(fileContent, userInputData)
                    .GetAwaiter()
                    .GetResult()
                    .Where(result => !string.IsNullOrWhiteSpace(result.ErrorMessage));

                if (resultsWithErrors.Any())
                {
                    Console.WriteLine(" ");
                    Console.Error.WriteLine("--------------ERRORS---------------");
                    Console.WriteLine(" ");

                    foreach (var result in resultsWithErrors)
                    {
                        Console.Error.WriteLine($"Id: {result.RecordId} | Error: {result.ErrorMessage}");
                        Console.WriteLine(" ");
                    }
                    Console.WriteLine(" ");
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("  ");
            Console.WriteLine("ALL DONE!");
        }

        static IConfiguration LoadConfigurations() 
        {
            Console.WriteLine("  ");
            Console.WriteLine("Loading appsettings configs...");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Console.WriteLine("Loaded!");

            return configuration;
        }

        static UserInputData GetValidUserInputs()
        {
            var userInputData = new UserInputService()
                .GetInputData();

            Console.WriteLine("  ");
            Console.WriteLine("Validating inputs...");
            userInputData.ThrowErrorIfInvalid();
            Console.WriteLine("Validated!");
            Console.WriteLine("  ");

            return userInputData;
        }
    }
}
