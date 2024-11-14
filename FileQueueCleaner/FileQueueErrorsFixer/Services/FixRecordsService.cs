using FileQueueErrorsFixer.Models;
using FileQueueErrorsFixer.Models.FileQueue;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Extensions.Configuration;

namespace FileQueueErrorsFixer.Services
{
    internal class FixRecordsService
    {
        private const string DatabaseNameConfigKey = "CosmosDb:DatabaseName";
        private const string CollectionNameConfigKey = "CosmosDb:CollectionName";
        private const string AzureContainerNameConfigKey = "AzureStorage:ContainerName";

        private readonly IConfiguration configuration;

        private readonly string cosmosDatabaseName;
        private readonly string cosmosCollectionName;
        private readonly string azureStorageContainerName;

        public FixRecordsService(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.cosmosDatabaseName = configuration[DatabaseNameConfigKey] ?? string.Empty;
            this.cosmosCollectionName = configuration[CollectionNameConfigKey] ?? string.Empty;
            this.azureStorageContainerName = configuration[AzureContainerNameConfigKey] ?? string.Empty;
        }

        public async Task<IEnumerable<FixRecordResult>> FixRecordsAsync(IEnumerable<FileQueue> recordsToFix, UserInputData userInputData) 
        {
            var cosmosDbService = new FileQueueCosmosDbService(
                userInputData.CosmosDbConnectionString,
                cosmosDatabaseName,
                cosmosCollectionName);

            var azureStorageService = new AzureStorageService(userInputData);
            var results = new List<FixRecordResult>();

            foreach (var record in recordsToFix) 
            {
                try
                {
                    var (packages, errorMessage) = await cosmosDbService.GetPackagesFileQueueAsync(record);

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        throw new Exception(errorMessage);
                    }

                    var archiveFolder = record.FilePath;
                    var recordArchiveFilePath = record.GetCurrentFilePathWithFileName();
                    var recordReviewQueueFilePath = record.GetOriginalReviewQueuePath();

                    record.UpdatePropertiesAndStatusToPending(recordReviewQueueFilePath);

                    // Moving parent file from archive to ReviewQueue folder
                    await ExecuteAndThrowIfErrorAsync(() => azureStorageService
                        .MoveFileAsync(azureStorageContainerName, recordArchiveFilePath, $"{recordReviewQueueFilePath}/{record.FileName}"));

                    // Updating parent and packages' FileQueues
                    await ExecuteAndThrowIfErrorAsync(() => cosmosDbService
                        .UpdateParentFileQueueAndDeletePackagesFileQueuesAsync(record, packages?.Select(item => item.Id) ?? new List<string>()));

                    // Delete all package's files
                    await ExecuteAndThrowIfErrorAsync(() => azureStorageService
                        .DeleteMultipleBlobsAsync(azureStorageContainerName, packages?.Select(fileQueue => (fileQueue.FilePath, fileQueue.FileName)) ?? new List<(string, string)>()));

                    // Deleting archive directory
                    await ExecuteAndThrowIfErrorAsync(() => azureStorageService
                        .DeleteBlobAsync(azureStorageContainerName, archiveFolder));

                    results.Add(new(record.Id, string.Empty));
                }
                catch (Exception e) 
                {
                    results.Add(new(record.Id, e.Message));
                }
            }

            return results;
        }

        private async Task ExecuteAndThrowIfErrorAsync(Func<Task<(bool result, string errorMessage)>> funcToExecute) 
        {
            var (result, errorMessage) = await funcToExecute();

            if (!result) 
            {
                throw new Exception(errorMessage);
            }
        }

        private async Task ExecuteAndThrowIfErrorAsync<T>(Func<Task<(T? result, string errorMessage)>> funcToExecute)
        {
            var (result, errorMessage) = await funcToExecute();

            if (result is null)
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
