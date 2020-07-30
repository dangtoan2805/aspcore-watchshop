using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace aspCore.WatchShop.Helper
{
    public static class SessionHelper
    {
        public static void SetData(this ISession session, string key, object data)
        {
            session.SetString(key, JsonConvert.SerializeObject(data));
        }

        public static T GetData<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}