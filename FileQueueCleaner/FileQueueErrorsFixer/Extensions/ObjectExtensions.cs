using Newtonsoft.Json;

namespace FileQueueErrorsFixer.Extensions
{
    internal static class ObjectExtensions
    {
        public static string ToJsonString(this object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
