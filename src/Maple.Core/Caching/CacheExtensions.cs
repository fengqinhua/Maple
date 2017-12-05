using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maple.Core.Caching
{
    /// <summary>
    /// 缓存管理器的扩展方法
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 获取缺省的缓存时间（默认为60分钟）
        /// </summary>
        private static int DefaultCacheTimeMinutes { get { return 60; } }

        /// <summary>
        /// 根据Key从缓存管理器中获取缓存的值，如果缓存不存在则创建之
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="cacheManager">缓存管理器</param>
        /// <param name="key">缓存的Key</param>
        /// <param name="acquire">创建缓存内容的方法</param>
        /// <returns>缓存的内容</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            //默认使用缺省时间
            return Get(cacheManager, key, DefaultCacheTimeMinutes, acquire);
        }

        /// <summary>
        /// 根据Key从缓存管理器中获取缓存的值，如果缓存不存在则创建之
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="cacheManager">缓存管理器</param>
        /// <param name="key">缓存的Key</param>
        /// <param name="cacheTime">缓存时间，如果小于0则将不缓存数据</param>
        /// <param name="acquire">创建缓存内容的方法</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            //如果缓存已经存在，则返回缓存值
            if (cacheManager.IsSet(key))
                return cacheManager.Get<T>(key);

            //如果缓存不存在，则创建缓存的内容
            var result = acquire();

            //添加缓存（根据缓存失效时间）
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);

            return result;
        }

        /// <summary>
        /// 按照正则表达式的规则从缓存管理器中移除缓存
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="pattern">Pattern</param>
        /// <param name="keys">All keys in the cache</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            //根据正则表达式获取缓存的Key集合
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();

            //移除匹配的缓存内容
            matchesKeys.ForEach(cacheManager.Remove);
        }
    }
}
