#nullable enable

using FileQueueErrorsFixer.Constants;
using FileQueueErrorsFixer.Interfaces;
using Newtonsoft.Json;

namespace FileQueueErrorsFixer.Models.FileQueue.Actions
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
