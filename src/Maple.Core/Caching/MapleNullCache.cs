﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Core.Caching
{
    /// <summary>
    /// 无效的缓存管理器
    /// </summary>
    public partial class MapleNullCache : IStaticCacheManager
    {
        /// <summary>
        /// 根据Key从缓存管理器中获取缓存的值.
        /// </summary>
        /// <typeparam name="T">缓存值的类型</typeparam>
        /// <param name="key">缓存的Key</param>
        /// <returns>缓存的Key所对应的值</returns>
        public virtual T Get<T>(string key)
        {
            return default(T);
        }

        /// <summary>
        /// 在缓存管理器中添加缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <param name="data">缓存的内容</param>
        /// <param name="cacheTime">缓存的过期时间</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
        }

        /// <summary>
        /// 查询某一key在缓存管理器中是否有缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        /// <returns>如果存在则返回True ; 否则为 false</returns>
        public bool IsSet(string key)
        {
            return false;
        }

        /// <summary>
        /// 根据Key从缓存管理器中移除缓存
        /// </summary>
        /// <param name="key">缓存的Key</param>
        public virtual void Remove(string key)
        {
        }

        /// <summary>
        /// 按照正则表达式的规则从缓存管理器中移除缓存
        /// </summary>
        /// <param name="pattern">规则</param>
        public virtual void RemoveByPattern(string pattern)
        {
        }

        /// <summary>
        /// 从缓存管理器中移除所有缓存
        /// </summary>
        public virtual void Clear()
        {
        }


        public virtual void Dispose()
        {

        }
    }
}
