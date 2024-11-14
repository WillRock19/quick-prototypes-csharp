#nullable enable

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using FileQueueErrorsFixer.Models;

namespace FileQueueErrorsFixer.Services
{
    internal class AzureStorageService
    {
        private readonly string connectionString;

        public AzureStorageService(UserInputData userInputData)
        {
            connectionString = userInputData.AzureStorageConnectionString;
        }

        public async Task<bool> UploadFileToContainer(FileStream fileToUpload, string containerName, string filePathToSave)
        {
            ArgumentNullException.ThrowIfNull(fileToUpload, nameof(fileToUpload));
            ArgumentNullException.ThrowIfNullOrEmpty(containerName, nameof(containerName));
            ArgumentNullException.ThrowIfNullOrEmpty(filePathToSave, nameof(filePathToSave));

            try
            {
                // Create a BlobServiceClient
                var blobServiceClient = new BlobServiceClient(connectionString);

                // Get container
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Get a reference to a blob
                var blobClient = containerClient.GetBlobClient(filePathToSave);

                // Upload the file
                var result = await blobClient.UploadAsync(fileToUpload, true);

                fileToUpload.Close();

                return result.GetRawResponse().IsError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OPERATION FAILED: Could not upload file '{Path.GetFileName(filePathToSave)}'. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool result, string errorMessage)> MoveFileAsync(string containerName, string fromFilePath, string toFilePath)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName), "containerName is null or empty.");
            }

            if (string.IsNullOrEmpty(fromFilePath))
            {
                throw new ArgumentNullException(nameof(fromFilePath), "fromFilePath is empty");
            }

            if (string.IsNullOrEmpty(toFilePath))
            {
                throw new ArgumentNullException(nameof(toFilePath), "toFilePath is null or empty.");
            }

            // Create a BlobServiceClient
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get container
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob
            var sourceFileBlobClient = containerClient.GetBlobClient(fromFilePath);
            var targetFileBlobClient = containerClient.GetBlobClient(toFilePath);

            if (!(await sourceFileBlobClient.ExistsAsync()).Value)
            {
                return PrintAndCreateErrorResult($"[ERROR - {nameof(MoveFileAsync)}]: File {fromFilePath} does not exist on Azure Storage.");
            }

            // Copying file from source to targer
            var (copyResult, copyErrorMessage) = await CopyFileBetweenSourceAndTargetContainersAsync(
                sourceFileBlobClient, 
                targetFileBlobClient);

            if (!copyResult)
            {
                return PrintAndCreateErrorResult($"[ERROR - {nameof(CopyFileBetweenSourceAndTargetContainersAsync)}]: Copy from '{sourceFileBlobClient.Uri}' into '{targetFileBlobClient.Uri}' failed. Exception: {copyErrorMessage}.");
            }

            // Deleting original file
            var (deleteFileResult, deleteFileErrorMessage) = await DeleteBlobAsync(sourceFileBlobClient);

            if (!deleteFileResult)
            {
                return PrintAndCreateErrorResult($"[ERROR - {nameof(DeleteBlobAsync)}]: Deletion of '{sourceFileBlobClient.Uri}' failed. Exception: {deleteFileErrorMessage}.");
            }

            return (true, string.Empty);
        }

        public async Task<(bool result, string errorMessage)> DeleteBlobAsync(string containerName, string blobPath) 
        {
            // Create a BlobServiceClient
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get container
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobPath);

            // Deleting original file's container
            var (deleteResult, deleteErrorMessage) = await DeleteBlobAsync(blobClient);

            if (!deleteResult)
            {
                return PrintAndCreateErrorResult($"[ERROR - {nameof(DeleteBlobAsync)}]: Deletion of '{blobClient.Uri}' failed. Exception: {deleteErrorMessage}.");
            }

            return (true, string.Empty);
        }

        public async Task<(bool result, string errorMessage)> DeleteMultipleBlobsAsync(string containerName, IEnumerable<(string filePath, string fileName)> filePathsWithNames)
        {
            try
            {
                // Create a BlobServiceClient
                var blobServiceClient = new BlobServiceClient(connectionString);

                // Get container
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Create batch client
                var batchClient = containerClient.GetBlobBatchClient();

                var urisToDelete = filePathsWithNames
                    .Select(pathAndName => new Uri($"{containerClient.GetBlobClient(pathAndName.filePath).Uri}/{pathAndName.fileName}"));

                await batchClient.DeleteBlobsAsync(urisToDelete);
                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        private async Task<(bool result, string errorMessage)> CopyFileBetweenSourceAndTargetContainersAsync(BlobClient sourceClient, BlobClient targetClient) 
        {
            try
            {
                await targetClient.StartCopyFromUriAsync(sourceClient.Uri);
                await sourceClient.DeleteIfExistsAsync();
                return (true, string.Empty);
            }
            catch (Exception e) 
            {
                return (false, e.Message);
            }
        }

        private async Task<(bool result, string errorMessage)> DeleteBlobAsync(BlobClient blobClient)
        {
            try
            {
                await blobClient.DeleteIfExistsAsync();
                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        private (bool result, string errorMessage) PrintAndCreateErrorResult(string errorMessage) 
        {
            Console.Error.WriteLine(errorMessage);
            return (false, errorMessage);
        }
    }
}
