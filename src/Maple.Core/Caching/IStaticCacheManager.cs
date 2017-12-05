using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Core.Caching
{
    /// <summary>
    /// 长期缓存管理器
    /// <para>相对于PerRequestCacheManager（仅某一次Http请求有效）</para>
    /// </summary>
    public interface IStaticCacheManager : ICacheManager
    {
    }
}
