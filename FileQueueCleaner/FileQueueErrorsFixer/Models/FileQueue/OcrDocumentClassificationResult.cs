using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileQueueCleanerConsoleApp.Models.FileQueue
{
    public class OcrDocumentClassificationResult
    {
        [JsonProperty("documentType")]
        public string DocumentType { get; init; }

        [JsonProperty("pages")]
        public IEnumerable<int> Pages { get; init; }

        [JsonProperty("confidenceLevel")]
        public float ConfidenceLevel { get; init; }
    }
}
