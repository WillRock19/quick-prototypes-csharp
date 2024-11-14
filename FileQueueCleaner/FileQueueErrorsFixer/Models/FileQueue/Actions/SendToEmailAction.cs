#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public class SendToEmailAction : IPackageActionType
    {
        [JsonProperty("kind")]
        public string Kind => PackageActionTypeKind.SendToEmail;

        [JsonProperty("actionName")]
        public string ActionName
        {
            get => "Send to Email";
            init => _ = value;
        }

        [JsonProperty("actionData")]
        public string ActionData { get; init; } = string.Empty;

        public static SendToEmailAction CreateInstance(string actionData = "") => new()
        {
            ActionData = actionData,
        };

        public bool HasActionDataProperty() => true;
    }
}
