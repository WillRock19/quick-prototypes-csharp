#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public record SendEmailToDepartmentAction : IPackageActionType
    {
        [JsonProperty("kind")]
        public string Kind => PackageActionTypeKind.SendEmailToDepartment;

        [JsonProperty("actionName")]
        public string ActionName { get; init; } = string.Empty;

        public static SendEmailToDepartmentAction CreateInstanceForSendToCob() => new() { ActionName = "Send to COB department" };

        public static SendEmailToDepartmentAction CreateInstanceForSendToDer() => new() { ActionName = "Send to DER department" };

        public static SendEmailToDepartmentAction CreateInstanceForSendToEligibility() => new() { ActionName = "Send to ELIGIBILITY department" };

        public static SendEmailToDepartmentAction CreateInstanceForSendToCancellation() => new() { ActionName = "Send to CANCELLATION department" };

        public static SendEmailToDepartmentAction CreateInstanceForSendToReinstatement() => new() { ActionName = "Send to REINSTATEMENT department" };

        public static SendEmailToDepartmentAction CreateInstanceForSendToMembership() => new() { ActionName = "Send to MEMBERSHIP department" };

        public bool HasActionDataProperty() => false;
    }
}
