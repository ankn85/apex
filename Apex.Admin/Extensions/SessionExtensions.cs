using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Apex.Admin.Extensions
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, JsonSettings));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ?
                default(T) :
                JsonConvert.DeserializeObject<T>(value, JsonSettings);
        }
    }
}
