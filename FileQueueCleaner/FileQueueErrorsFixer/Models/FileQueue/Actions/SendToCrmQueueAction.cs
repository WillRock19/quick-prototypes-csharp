#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public record SendToCrmQueueAction : IPackageActionType
    {
        [JsonProperty("kind")]

        public string Kind => PackageActionTypeKind.SendToCrm;

        [JsonProperty("actionName")]
        public string ActionName { get; init; } = string.Empty;

        [JsonProperty("actionData")]
        public string ActionData { get; init; } = string.Empty;

        [JsonProperty("selectedCrmQueue")]
        public string SelectedCrmQueue { get; init; } = string.Empty;

        public bool HasActionDataProperty() => true;
    }
}
