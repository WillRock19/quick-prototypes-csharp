using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileQueueErrorsFixer.Models
{
    public class CosmosDbEntity
    {
        /// <summary>
        /// All the types inheriting from this type and overriding the following
        /// Id property needs to add JsonProperty("id") attribute to the override
        /// as well.
        /// This makes sure that linq to sql will pick up the all lowercase "id"
        /// property when generating sql queries. Since cosmosdb queries are case
        /// sensitive, "Id" would not match to "id".
        /// </summary>
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        public virtual string PartitionKey { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }

        [JsonProperty("_ts")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual string CreatedById { get; set; }

        public virtual string CreatedByName { get; set; }

        public virtual DateTimeOffset CreatedAt { get; set; }

        public virtual string LastUpdatedById { get; set; }

        public virtual string LastUpdatedByName { get; set; }

        public virtual DateTimeOffset LastUpdatedAt { get; set; }
    }
}
