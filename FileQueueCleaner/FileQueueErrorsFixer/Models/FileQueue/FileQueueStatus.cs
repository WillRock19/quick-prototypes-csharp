namespace FileQueueErrorsFixer.Models.FileQueue
{
    public enum FileQueueStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Downloaded = 3,
        EnqueuedToCompletePackages = 4,
        ErrorDuringProcessing = 5,
        Skipped = int.MaxValue,
    }
}
