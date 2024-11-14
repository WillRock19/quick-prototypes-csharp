using FileQueueErrorsFixer.Extensions;
using FileQueueErrorsFixer.Interfaces;
using FileQueueErrorsFixer.Models.FileQueue.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileQueueErrorsFixer.Converters
{
    internal class PackageActionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPackageActionType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jsonObject = JObject.Load(reader);
                var kind = jsonObject.TryGetValue(nameof(IPackageActionType.Kind), StringComparison.OrdinalIgnoreCase, out var propertyValue);

                var propertyAsString = propertyValue?.Value<string>() ?? string.Empty;

                if (string.IsNullOrEmpty(propertyAsString))
                {
                    return null;
                }

                using var stringReader = new StringReader(jsonObject.ToString());
                using var jsonReader = new JsonTextReader(stringReader);

                var type = this.GetTypeForKind(propertyAsString);
                return serializer.Deserialize(jsonReader, type);
            }
            catch
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                var jsonObject = JObject.Parse(value.ToJsonString());
                var kindValue = jsonObject.TryGetValue(nameof(IPackageActionType.Kind), StringComparison.OrdinalIgnoreCase, out var propertyValue);
                var propertyAsString = propertyValue?.Value<string>() ?? string.Empty;

                if (string.IsNullOrEmpty(propertyAsString))
                {
                    serializer.Serialize(writer, null, typeof(string));
                }
                else
                {
                    var objectType = this.GetTypeForKind(propertyAsString);
                    serializer.Serialize(writer, value, objectType);
                }
            }
            catch
            {
                serializer.Serialize(writer, null, typeof(string));
            }
        }

        private Type GetTypeForKind(string kindValue) => kindValue switch
        {
            nameof(CreateEnrollmentAction) => typeof(CreateEnrollmentAction),
            nameof(SendToCrmQueueAction) => typeof(SendToCrmQueueAction),
            nameof(SendToEmailAction) => typeof(SendToEmailAction),
            nameof(SendEmailToDepartmentAction) => typeof(SendEmailToDepartmentAction),
            _ => typeof(DiscardAction),
        };
    }
}
