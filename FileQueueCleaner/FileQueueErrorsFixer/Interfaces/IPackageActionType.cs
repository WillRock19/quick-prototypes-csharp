#nullable enable

namespace FileQueueErrorsFixer.Interfaces
{
    public interface IPackageActionType
    {
        /// <summary>
        /// Defines the actual type of the PackageAction.
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Defines the actual PackageAction's name.
        /// </summary>
        string ActionName { get; init; }

        /// <summary>
        /// Since some of the PackageAction might contain a ActionData property, this method will allow any caller to understand if this is the case.
        /// IMPORTANT: this method does not consider if the ActionData property has a value, only if the property exists on the concrete class.
        /// </summary>
        bool HasActionDataProperty();
    }
}
