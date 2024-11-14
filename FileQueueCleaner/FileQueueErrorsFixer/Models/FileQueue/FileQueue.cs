#nullable enable


using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace FileQueueErrorsFixer.Models.FileQueue
{
    public class FileQueue : CosmosDbEntity
    {
        [JsonProperty("fileName")]
        public string FileName { get; init; } = string.Empty;

        [JsonProperty("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonProperty("fileOrigin")]
        public string FileOrigin { get; init; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonProperty("packages")]
        public IList<ClassificationPackage> Packages { get; set; } = new List<ClassificationPackage>();

        [JsonProperty("parentId")]
        public string ParentId { get; init; } = string.Empty;

        [JsonProperty("schemaName")]
        public string SchemaName { get; init; } = "FileQueue";

        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; init; } = "1.1";

        [JsonProperty("documentType")]
        public string DocumentType { get; init; } = string.Empty;

        [JsonProperty("uploadedBy")]
        public string UploadedBy { get; init; } = string.Empty;

        [JsonProperty("uploadedAt")]
        public DateTimeOffset? UploadedAt { get; init; }

        [JsonProperty("reviewedBy")]
        public string ReviewedBy { get; init; } = string.Empty;

        [JsonProperty("reviewedAt")]
        public DateTimeOffset? ReviewedAt { get; init; }

        [JsonProperty("packagesCompletedAt")]
        public DateTimeOffset? PackagesCompletionSubmittedAt { get; init; }

        [JsonProperty("packagesCompletedBy")]
        public string? PackagesCompletionSubmittedBy { get; init; }

        public string DateWhenFileQueueWasCreatedAsString() => PartitionKey;

        public string GetCurrentFilePathWithFileName() => $"{FilePath}/{FileName}";

        public string GetOriginalReviewQueuePath() => $"ReviewQueue/{PartitionKey}/{FromFolderName()}";

        public void UpdatePropertiesAndStatusToPending(string filePath) 
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
            
            FilePath = filePath;
            Status = FileQueueStatus.Pending.ToString();

            Packages = Packages
                .Select(package => package with 
                { 
                    Status = ClassificationPackageStatus.Created.ToString(), 
                    CompletionErrorMessage = string.Empty 
                })
                .ToList();
        }

        public bool isValid() => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Etag);

        public List<PatchOperation> GetPathOperationsToUpdate() 
        {
            var patchOperations = new List<PatchOperation>() 
            {
                PatchOperation.Set($"/filePath", FilePath),
                PatchOperation.Set($"/status", Status),
                PatchOperation.Set($"/notes", string.Empty),
            };

            for (var index = 0; index < Packages.Count; index++) 
            {
                var packageUpdate = Packages.ElementAt(index);
                patchOperations.Add(PatchOperation.Set($"/packages/{index}/status", ClassificationPackageStatus.Created.ToString()));
                patchOperations.Add(PatchOperation.Set($"/packages/{index}/completionErrorMessage", string.Empty));
            }

            return patchOperations;
        }

        private string FromFolderName() => FileOrigin switch
        {
            "Email" => "FromEmail",
            "Fax Server" => "FromFax",
            "FTP Server" => "FromFtp",
            "Web Form" => "FromWebForm",
            "Mobile App" => "FromMobileApp",
            _ => throw new Exception($"The {nameof(FileQueue)} with id '{Id}' does not contain a valid FileOrigin value (value: {FileOrigin}).")
        };
    }
}
