using Newtonsoft.Json;

namespace FileQueueCleanerConsoleApp.Extensions
{
    internal static class ObjectExtensions
    {
        public static string ToJsonString(this object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
