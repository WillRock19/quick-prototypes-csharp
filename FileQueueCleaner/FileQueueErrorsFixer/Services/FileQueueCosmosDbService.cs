using FileQueueErrorsFixer.Models.FileQueue;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FileQueueErrorsFixer.Services
{
    internal class FileQueueCosmosDbService
    {
        public string DatabaseName { get; }
        public string ContainerName { get; }

        private readonly CosmosClient cosmosClient;

        private readonly Database database;

        private readonly Container container;

        internal FileQueueCosmosDbService(string connectionString, string databaseName, string containerName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
            ArgumentException.ThrowIfNullOrWhiteSpace(databaseName, nameof(databaseName));
            ArgumentException.ThrowIfNullOrWhiteSpace(containerName, nameof(containerName));

            DatabaseName = databaseName;
            ContainerName = containerName;

            cosmosClient = new CosmosClient(connectionString);
            database = cosmosClient.GetDatabase(DatabaseName);
            container = database.GetContainer(ContainerName);
        }

        public async Task<(List<FileQueue>? records, string errorMessage)> GetPackagesFileQueueAsync(FileQueue fileQueue) 
        {
            try 
            {
                var query = $"SELECT * FROM c WHERE c.parentId = '{fileQueue.Id}'";
                var queryable = container.GetItemLinqQueryable<FileQueue>()
                        .Where(item => item.ParentId == fileQueue.ParentId);

                var queryDefinition = new QueryDefinition(query);
                var iterator = container.GetItemQueryIterator<FileQueue>(queryDefinition);

                var results = new List<FileQueue>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }

                return (results, string.Empty);
            }
            catch (Exception e)
            {
                var errorMessage = $"[ERROR - {nameof(UpdateParentFileQueueAndDeletePackagesFileQueuesAsync)}]: {e.Message}";
                return (null, errorMessage);
            }
        }

        public async Task<(FileQueue? record, string errorMessage)> UpdateParentFileQueueAndDeletePackagesFileQueuesAsync(
            FileQueue updatedParentFileQueue, 
            IEnumerable<string> packagesFileQueueIds) 
        {
            try
            {
                var partitionKey = new PartitionKey(updatedParentFileQueue.PartitionKey);
                var batch = container.CreateTransactionalBatch(partitionKey);

                foreach (var id in packagesFileQueueIds)
                {
                    batch = batch.DeleteItem(id);
                };

                var response = await batch
                    .UpsertItem(updatedParentFileQueue)
                    .ExecuteAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"=> Updated records associated to Parent FileQueue: {updatedParentFileQueue.Id}.");
                    return (updatedParentFileQueue, string.Empty);
                }
                else
                {
                    var errorMessage = $"Updated records associated to Parent FileQueue failed with status code: {response.StatusCode} and error '{response.ErrorMessage}'.";
                    Console.WriteLine($"=> {errorMessage}");
                    return (null, errorMessage);
                }
            }
            catch (Exception e) 
            {
                var errorMessage = $"[ERROR - {nameof(UpdateParentFileQueueAndDeletePackagesFileQueuesAsync)}]: {e.Message}";
                Console.Error.Write($"{errorMessage}");
                return (null, errorMessage);
            }
        }
    }
}
