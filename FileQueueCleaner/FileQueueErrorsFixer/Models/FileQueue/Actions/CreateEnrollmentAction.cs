#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public record CreateEnrollmentAction : IPackageActionType
    {
        [JsonProperty("kind")]
        public string Kind => PackageActionTypeKind.CreateEnrollment;

        [JsonProperty("actionName")]
        public string ActionName
        {
            get => "Create Enrollment Application";
            init => _ = value;
        }

        public static CreateEnrollmentAction CreateInstance() => new();

        public bool HasActionDataProperty() => false;
    }
}
