using FileQueueCleanerConsoleApp.Models;
using FileQueueCleanerConsoleApp.Models.FileQueue;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Services
{
    internal class FileLoaderService
    {
        internal async Task<(IEnumerable<FileQueue>?, string errorMessage)> LoadFileDataAsync(UserInputData userInputData) 
        {
            try
            {
                var fileJsonString = await File
                    .ReadAllTextAsync(userInputData.PathToJsonFileContainingFileQueuesToFix);

                if (string.IsNullOrWhiteSpace(fileJsonString))
                {
                    return (Enumerable.Empty<FileQueue>(), $"Content of file {userInputData.PathToJsonFileContainingFileQueuesToFix} is empty.");
                }

                return (JsonConvert.DeserializeObject<IEnumerable<FileQueue>>(fileJsonString!), string.Empty);
            }
            catch (Exception e) 
            {
                var errorMessage = $"[ERROR - {nameof(LoadFileDataAsync)}]: {e.Message}";
                Console.Error.WriteLine(errorMessage);
                return (Enumerable.Empty<FileQueue>(), errorMessage);
            }
        }
    }
}
