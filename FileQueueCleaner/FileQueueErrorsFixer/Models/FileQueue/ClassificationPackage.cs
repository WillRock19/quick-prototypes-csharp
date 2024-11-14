﻿#nullable enable

using FileQueueErrorsFixer.Converters;
using FileQueueErrorsFixer.Interfaces;
using FileQueueErrorsFixer.Models.FileQueue.Actions;
using Newtonsoft.Json;

namespace FileQueueErrorsFixer.Models.FileQueue
{
    public record ClassificationPackage
    {
        [JsonProperty("status")]
        public string Status { get; init; } = ClassificationPackageStatus.Created.ToString();

        [JsonProperty("completionErrorMessage")]
        public string? CompletionErrorMessage { get; init; }

        [JsonProperty("packageNumber")]
        public int PackageNumber { get; init; }

        [JsonProperty("classificationResults")]
        public IList<OcrDocumentClassificationResult> ClassificationResults { get; init; }

        [JsonProperty("pages")]
        public IList<string> Pages { get; init; }

        [JsonProperty("actionType")]
        [JsonConverter(typeof(PackageActionJsonConverter))]
        public IPackageActionType ActionType { get; init; } = new UndefinedAction();
    }
}