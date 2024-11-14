using FileQueueCleanerConsoleApp.Models.FileQueue.Actions;

namespace FileQueueCleanerConsoleApp.Constants
{
    public class PackageActionTypeKind
    {
        public static string Discard => nameof(DiscardAction);

        public static string CreateEnrollment => nameof(CreateEnrollmentAction);

        public static string SendToCrm => nameof(SendToCrmQueueAction);

        public static string SendToEmail => nameof(SendToEmailAction);

        public static string SendEmailToDepartment => nameof(SendEmailToDepartmentAction);

        public static string Undefined => nameof(UndefinedAction);
    }
}
