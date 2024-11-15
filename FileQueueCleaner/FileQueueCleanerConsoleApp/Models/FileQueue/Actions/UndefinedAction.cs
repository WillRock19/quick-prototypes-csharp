﻿#nullable enable

using FileQueueCleanerConsoleApp.Constants;
using FileQueueCleanerConsoleApp.Interfaces;
using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Models.FileQueue.Actions
{
    public class UndefinedAction : IPackageActionType
    {
        [JsonProperty("kind")]
        public string Kind => PackageActionTypeKind.Undefined;

        [JsonProperty("actionName")]
        public string ActionName
        {
            get => "This action exists as default (a valid one wasn't retrieved)";
            init => _ = value;
        }

        public static UndefinedAction CreateInstance() => new();

        public bool HasActionDataProperty() => false;
    }
}
