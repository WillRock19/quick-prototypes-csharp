#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public record DiscardAction : IPackageActionType
    {
        [JsonProperty("kind")]
        public string Kind => PackageActionTypeKind.Discard;

        [JsonProperty("actionName")]
        public string ActionName
        {
            get => "Discard";
            init => _ = value;
        }

        public static DiscardAction CreateInstance() => new();

        public bool HasActionDataProperty() => false;
    }
}
