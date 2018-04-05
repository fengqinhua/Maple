using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maple.Core.Caching
{
    /// <summary>
    /// 基于内存的缓存管理器
    /// </summary>
    public partial class MemoryCacheManager : IStaticCacheManager
    {
        #region 字段

        private readonly IMemoryCache _cache;

        /// <summary>
        /// 缓存失效信号源
        /// </summary>
        protected CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 所有已缓存数据的Key
        /// </summary>
        /// <remarks></remarks> 
        protected static readonly ConcurrentDictionary<string, bool> _allKeys;

        #endregion

        #region 构造函数

        /// <summary>
        /// 基于内存的缓存管理器
        /// </summary>
        static MemoryCacheManager()
        {
            _allKeys = new ConcurrentDictionary<string, bool>();
        }

        /// <summary>
        /// 基于内存的缓存管理器
        /// </summary>
        /// <param name="cache">Cache</param>
        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region 非公共方法

        /// <summary>
        /// 创建缓存失效规则
        /// </summary>
        /// <param name="cacheTime">失效时长（分钟）</param>
        /// <returns></returns>
        protected MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                //添加缓存失效规则：所有缓存均依赖_cancellationTokenSource，可以通过_cancellationTokenSource让所有缓存失效
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                //添加缓存失效回掉函数：在从缓存中清除缓存条目后，将触发给定的回调。
                .RegisterPostEvictionCallback(PostEviction);

            //添加缓存失效时间
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime);

            return options;
        }

        /// <summary>
        /// 添加Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        /// <summary>
        /// 移除Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string RemoveKey(string key)
        {
            TryRemoveKey(key);
            return key;
        }

        /// <summary>
        /// 移除key，如果移除失败，那么将key标识为失效
        /// </summary>
        /// <param name="key"></param>
        protected void TryRemoveKey(string key)
        {
            //try to remove key from dictionary
            if (!_allKeys.TryRemove(key, out bool _))
                //if not possible to remove key from dictionary, then try to mark key as not existing in cache
                _allKeys.TryUpdate(key, false, false);
        }

        /// <summary>
        /// 移除所有状态为失效的Key
        /// </summary>
        private void ClearKeys()
        {
            foreach (var key in _allKeys.Where(p => !p.Value).Select(p => p.Key).ToList())
            {
                RemoveKey(key);
            }
        }

        /// <summary>
        /// 在从缓存中清除缓存条目后，将触发给定的回调。
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <param name="value">缓存的值</param>
        /// <param name="reason">失效原因</param>
        /// <param name="state">状态</param>
        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            //如果缓存变化，则不做任何操作
            if (reason == EvictionReason.Replaced)
                return;
            //移除所有状态为失效的Key
            ClearKeys();
            //移除当前Key
            TryRemoveKey(key.ToString());
        }

        #endregion

        #region IStaticCacheManager 实现

        /// <summary>
        /// 根据Key从缓存管理器中获取缓存的值.
        /// </summary>
        /// <typeparam name="T">缓存值的类型</typeparam>
        /// <param name="key">缓存的Key</param>
        /// <returns>缓存的Key所对应的值</returns>
        public virtual T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 在缓存管理器中添加缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <param name="data">缓存的内容</param>
        /// <param name="cacheTime">缓存的过期时间</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data != null)
            {
                _cache.Set(AddKey(key), data, GetMemoryCacheEntryOptions(cacheTime));
            }
        }

        /// <summary>
        /// 查询某一key在缓存管理器中是否有缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <returns>如果存在则返回True ; 否则为 false</returns>
        public virtual bool IsSet(string key)
        {
            return _cache.TryGetValue(key, out object _);
        }

        /// <summary>
        /// 根据Key从缓存管理器中移除缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        public virtual void Remove(string key)
        {
            _cache.Remove(RemoveKey(key));
        }

        /// <summary>
        /// 按照正则表达式的规则从缓存管理器中移除缓存
        /// </summary>
        /// <param name="pattern">规则</param>
        public virtual void RemoveByPattern(string pattern)
        {
            this.RemoveByPattern(pattern, _allKeys.Where(p => p.Value).Select(p => p.Key));
        }

        /// <summary>
        /// 从缓存管理器中移除所有缓存
        /// </summary>
        public virtual void Clear()
        {
            //发送让所有缓存失效的信号
            _cancellationTokenSource.Cancel();
            //让所有缓存失效
            _cancellationTokenSource.Dispose();
            //重新创建缓存失效信号源
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public virtual void Dispose()
        {
        }

        #endregion
    }
}