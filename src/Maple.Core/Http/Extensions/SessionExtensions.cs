using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Maple.Core.Http.Extensions
{
    /// <summary>
    /// ISession 的扩展方法
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// 保存对象至ISession中
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 从ISession中读取值
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="session">Session</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
