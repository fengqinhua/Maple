using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maple.Core.Caching
{
    /// <summary>
    /// 仅某一次Http请求生命周期内有效的缓存管理器
    /// </summary>
    public partial class PerRequestCacheManager : ICacheManager
    {
        #region 属性

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region 构造函数

        /// <summary>
        /// 仅某一次Http请求生命周期内有效的缓存管理器
        /// </summary>
        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region 非公共方法

        /// <summary>
        /// 获取一个可在某一次Http请求生命周期内共享使用的Key/Value集合
        /// </summary>
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
        }

        #endregion

        #region ICacheManager 实现

        /// <summary>
        /// 根据Key从缓存管理器中获取缓存的值.
        /// </summary>
        /// <typeparam name="T">缓存值的类型</typeparam>
        /// <param name="key">缓存的Key</param>
        /// <returns>缓存的Key所对应的值</returns>
        public virtual T Get<T>(string key)
        {
            var items = GetItems();
            if (items == null)
                return default(T);

            return (T)items[key];
        }

        /// <summary>
        /// 在缓存管理器中添加缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <param name="data">缓存的内容</param>
        /// <param name="cacheTime">缓存的过期时间</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            var items = GetItems();
            if (items == null)
                return;

            if (data != null)
                items[key] = data;
        }

        /// <summary>
        /// 查询某一key在缓存管理器中是否有缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <returns>如果存在则返回True ; 否则为 false</returns>
        public virtual bool IsSet(string key)
        {
            var items = GetItems();

            return items?[key] != null;
        }

        /// <summary>
        /// 根据Key从缓存管理器中移除缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        public virtual void Remove(string key)
        {
            var items = GetItems();

            items?.Remove(key);
        }

        /// <summary>
        /// 按照正则表达式的规则从缓存管理器中移除缓存
        /// </summary>
        /// <param name="pattern">规则</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;

            this.RemoveByPattern(pattern, items.Keys.Select(p => p.ToString()));
        }

        /// <summary>
        /// 从缓存管理器中移除所有缓存
        /// </summary>
        public virtual void Clear()
        {
            var items = GetItems();

            items?.Clear();
        }


        public virtual void Dispose()
        {
        }

        #endregion
    }
}
