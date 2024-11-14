#nullable enable

namespace FileQueueErrorsFixer.Models
{
    internal class UserInputData
    {
        public string PathToJsonFileContainingFileQueuesToFix { get; init; } = string.Empty;

        public string CosmosDbConnectionString { get; init; } = string.Empty;

        public string AzureStorageConnectionString { get; init; } = string.Empty;

        public void ThrowErrorIfInvalid() 
        {
            var propertiesWithCheckResult = new List<(string propertyName, bool isNullOrEmpty)>() 
            {
                (nameof(PathToJsonFileContainingFileQueuesToFix), string.IsNullOrWhiteSpace(PathToJsonFileContainingFileQueuesToFix)),
                (nameof(AzureStorageConnectionString), string.IsNullOrWhiteSpace(AzureStorageConnectionString)),
                (nameof(CosmosDbConnectionString), string.IsNullOrWhiteSpace(CosmosDbConnectionString)),
            };

            if (propertiesWithCheckResult.Any(item => item.isNullOrEmpty)) 
            {
                throw new InvalidOperationException($"Configurations '{string.Join(",", propertiesWithCheckResult.Select(x => x.propertyName))}' cannot be null or empty or empty string.");
            }
        }
    }
}
